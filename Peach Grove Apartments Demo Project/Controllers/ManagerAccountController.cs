﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PeachGroveApartments.ApplicationLayer.Interfaces;
using PeachGroveApartments.ApplicationLayer.ViewModels;
using PeachGroveApartments.Infrastructure.Data;
using PeachGroveApartments.Infrastructure.Identity;
using PeachGroveApartments.Infrastructure.Interfaces;
using PeachGroveApartments.Infrastructure.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Peach_Grove_Apartments_Demo_Project.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerAccountController : Controller
    {
        private readonly UserManager<AptUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private SignInManager<AptUser> _signInManager;
        private readonly IManagerRepository _managerRepository;
        private readonly IManagerLogic _managerLogic;
        private readonly ApplicationDbContext _context;

        public ManagerAccountController(ApplicationDbContext context,
            UserManager<AptUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper, SignInManager<AptUser> signInManager,
            IManagerRepository managerRepository,
            IManagerLogic managerLogic)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _signInManager = signInManager;
            _managerRepository = managerRepository;
            _managerLogic = managerLogic;
            _context = context;
        }

        // GET: ManagerAccount
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ShowApplicationUsers()
        {
            return View(await _managerRepository.GetApplicationUsers());
        }

        public async Task<IActionResult> ShowApplications(string userId)
        {
            return View(_mapper.Map<ApplicationViewModel>(await _managerRepository.GetApplications(userId)));
        }

        public async Task<IActionResult> ViewApplication(int Id)
        {
            var application = await _managerRepository.GetApplication(Id);
            return View(application);
        }

        public async Task<IActionResult> EditApplication(int Id)
        {
            var application = await _managerRepository.GetApplication(Id);
            if (application == null)
            {
                return NotFound();
            }

            var app = _mapper.Map<ApplicationViewModel>(application);

            ViewBag.AppEditSuccess = TempData["AppEditSuccess"];
            return View(app);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditApplication(ApplicationViewModel application)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var app = _mapper.Map<Application>(application);
                    await _managerRepository.EditApplication(app);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (ApplicationExists(application.ApplicationId) != null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                TempData["AppEditSuccess"] = true;
                return RedirectToAction("EditApplication");
            }

            ViewData["AptUserId"] = new SelectList(_context.AptUsers, "Id", "Id", application.AptUserId);
            return View(application);
        }

        // GET: ApplicantAccount/Delete/5
        public async Task<IActionResult> CancelApplication(int Id)
        {
            //var application = await _context.Applications
            //    .Include(a => a.AptUser)
            //    .FirstOrDefaultAsync(m => m.ApplicationId == id);

            var application = await _managerRepository.GetApplication(Id);

            if (application == null)
            {
                return NotFound();
            }

            return View(application);
        }

        // POST: ApplicantAccount/Delete/5
        [HttpPost, ActionName("AppCancel")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelApplicationConfirmed(Application app)
        {
            var application = await _managerLogic.CancelApplication(app.ApplicationId);

            return RedirectToAction("ShowApplications", new { userId = application.AptUserId });
        }

        public async Task<IActionResult> ApproveApplication(string userId, int applicationId, string ssn, string aptNumber, string aptPrice)
        {
            try
            {
                await _managerLogic.ApproveApplication(userId, applicationId, ssn, aptNumber, aptPrice);
            }
            catch (Exception e)
            {
                TempData["ApproveFailedMessage"] = e.Message;
                return RedirectToAction("ShowApproveApplicationFailed");
            }
            return RedirectToAction("ShowApproveApplicationSuccess");
        }

        public IActionResult ShowApproveApplicationSuccess()
        {
            return View();
        }

        public IActionResult ShowApproveApplicationFailed()
        {
            ViewBag.ApproveApplicationFailed = TempData["ApproveFailedMessage"];
            return View();
        }

        private async Task<Application> ApplicationExists(int id)
        {
            return await _managerRepository.GetApplication(id) ?? null;
        }

        public async Task<IActionResult> UnApproveApplication(string userId, string aptNumber, int applicationId)
        {
            try
            {
                await _managerLogic.UnApproveApplication(userId, aptNumber, applicationId);
            }
            catch (Exception e)
            {
                TempData["UnApproveFailedMessage"] = e.Message;
                return RedirectToAction("ShowUnApproveApplicationFailed");
            }
            return RedirectToAction("ShowUnApproveApplicationSuccess");
        }

        public IActionResult ShowUnApproveApplicationSuccess()
        {
            return View();
        }

        public IActionResult ShowUnApproveApplicationFailed()
        {
            ViewBag.UnApproveApplicationFailed = TempData["UnApproveFailedMessage"];
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ShowMaintenanceRequestsUsers()
        {
            return View(await _managerRepository.GetMaintenanceRequestsUsers());
        }

        public async Task<IActionResult> ShowMaintenanceRequests(string firstName, string lastName)
        {
            var MaintenanceRecords = await _managerRepository.GetMaintenanceUserRequests();

            var mViewModel = new MaintenanceRequestViewModel
            {
                mRequests = MaintenanceRecords,
                userFName = firstName,
                userLName = lastName
            };

            ViewBag.MaintenanceEditSuccess = TempData["MaintenanceEditSuccess"];
            return View(mViewModel);
        }

        public async Task<IActionResult> ViewMaintenanceRequest(int maintenanceId, string firstName, string lastName)
        {
            var maintenanceRecord = await _managerRepository.GetMaintenanceRequest(maintenanceId);

            if (maintenanceRecord == null)
            {
                return NotFound();
            }

            return View(new MaintenanceRequestViewModel
            {
                mRequest = maintenanceRecord,
                userFName = firstName,
                userLName = lastName
            });
        }

        public async Task<IActionResult> EditMaintenanceRequest(int maintenanceId, string firstName, string lastName)
        {
            var maintenanceRecord = await _managerRepository.GetMaintenanceRequest(maintenanceId);

            return View(new MaintenanceRequestViewModel { Id = maintenanceRecord.Id, AptUserId = maintenanceRecord.AptUserId, ProblemDescription = maintenanceRecord.ProblemDescription, isAllowedToEnter = maintenanceRecord.isAllowedToEnter, userFName = firstName, userLName = lastName });
        }

        // POST: ApplicantAccount/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditMaintenanceRequest(MaintenanceRequestViewModel maintenanceViewModel)
        {
            if (ModelState.IsValid)
            {
                await _managerLogic.EditMaintenanceRequest(maintenanceViewModel);
                TempData["MaintenanceEditSuccess"] = true;
                return RedirectToAction("ShowMaintenanceRequests", new { firstName = maintenanceViewModel.userFName, lastName = maintenanceViewModel.userLName });
            }

            //ViewData["AptUserId"] = new SelectList(_context.AptUsers, "Id", "Id", mViewModel.Id);

            return View(maintenanceViewModel);
        }

        public async Task<IActionResult> ApproveMaintenanceRequest(string userId, int maintenanceId)
        {
            try
            {
                await _managerLogic.ApproveMaintenanceRequest(userId, maintenanceId);
            }
            catch (Exception e)
            {
                TempData["ApproveFailedMessage"] = e.Message;
                return RedirectToAction("ShowApproveMaintenaceFailed");
            }

            return RedirectToAction("ShowApproveMaintenanceSuccess");
        }

        public IActionResult ShowApproveMaintenanceSuccess()
        {
            return View();
        }

        public IActionResult ShowApproveMaintenaceFailed()
        {
            ViewBag.ApproveMaintenanceFailed = TempData["ApproveFailedMessage"];
            return View();
        }

        private bool MaintenanceRequestExists(int id)
        {
            return _context.MaintenanceRequests.Any(e => e.Id == id);
        }

        public async Task<IActionResult> UnApproveMaintenanceRequest(string userId, int maintenanceId)
        {
            try
            {
                await _managerLogic.UnApproveMaintenanceRequest(userId, maintenanceId);
            }
            catch (Exception e)
            {
                TempData["UnApproveFailedMessage"] = e.Message;
                return RedirectToAction("ShowUnApproveMaintenanceFailed");
            }
            return RedirectToAction("ShowUnApproveMaintenanceSuccess");
        }

        public IActionResult ShowUnApproveMaintenanceSuccess()
        {
            return View();
        }

        public IActionResult ShowUnApproveMaintenanceFailed()
        {
            ViewBag.UnApproveMaintenanceFailed = TempData["UnApproveFailedMessage"];
            return View();
        }
    }
}