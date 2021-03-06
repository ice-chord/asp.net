﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace _20160410.Controllers
{
    public class OrderController : Controller
    {
        Models.codeService codeService = new Models.codeService();
        /// <summary>
        /// 訂單管理首頁
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            ViewBag.shipperCodeData = this.codeService.Getshipper();
            return View();
        }

        /// <summary>
        /// 取得訂單查詢結果
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult Index(Models.OrderSearchArg arg)
        {
            
            ViewBag.shipperCodeData = this.codeService.Getshipper();
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            Models.OrderService orderService = new Models.OrderService();
            ViewBag.SearchResult = orderService.GetOrderByCondtioin(arg);
            return View("Index");
        }


        /// <summary>
        /// 新增訂單畫面
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult InsertOrder()
        {
            ViewBag.CustCodeData = this.codeService.GetCustomer();
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            ViewBag.ProductCodeData = this.codeService.GetProduct();
            return View(new Models.Order());
        }

        /// <summary>
        /// 新增訂單
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPost()]
        public ActionResult InsertOrder(Models.Order order)
        {
            Models.OrderService OrderService = new Models.OrderService();
            OrderService.InsertOrder(order);
            return RedirectToAction("Index");
            //return View();
        }

        /// <summary>
        /// 更新訂單畫面
        /// </summary>
        /// <returns></returns>
        [HttpGet()]
        public ActionResult UpdateOrder()
        {
            ViewBag.CustCodeData = this.codeService.GetCustomer();
            ViewBag.EmpCodeData = this.codeService.GetEmp();
            ViewBag.ProductCodeData = this.codeService.GetProduct();
            return View(new Models.Order());
        }

        /// <summary>
        /// 更新訂單
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult UpdateOrder(Models.Order order)
        {
            return View();
        }

        /// <summary>
        /// 刪除訂單
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost()]
        public JsonResult DeleteOrder(string orderId)
        {
            try
            {
                Models.OrderService orderService = new Models.OrderService();
                orderService.DeleteOrderById(orderId);
                return this.Json(true);
            }
            catch (Exception)
            {

                return this.Json(false);
            }


        }

        /// <summary>
        /// 取得系統時間
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSysDate()
        {
            return PartialView("_SysDatePartial");
        }

    }
}
