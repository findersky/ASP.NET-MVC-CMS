﻿using Easy.Data;
using Easy.Web.CMS;
using Easy.Web.CMS.WidgetTemplate;
using Easy.Web.CMS.Zone;
using Easy.Web.Attribute;
using Easy.Web.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Easy.Extend;
using Easy.Constant;
using Easy.Web.CMS.Widget;

namespace Easy.CMS.Common.Controllers
{
    [AdminTheme]
    public class WidgetController : Controller
    {
        [ViewData_Zones]
        public ActionResult Create(QueryContext context)
        {
            var template = new WidgetTemplateService().Get(context.WidgetTemplateID);
            var widget = template.CreateWidgetInstance();
            widget.PageID = context.PageID;
            widget.LayoutID = context.LayoutID;
            widget.ZoneID = context.ZoneID;
            widget.Position = 1;
            ViewBag.ReturnUrl = context.ReturnUrl;            
            return View(widget);
        }
        [HttpPost, ViewData_Zones]
        [ValidateInput(false)]
        public ActionResult Create(WidgetBase widget, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(widget);
            }
            widget.CreateServiceInstance().AddWidget(widget);
            if (!ReturnUrl.IsNullOrEmpty())
            {
                return Redirect(ReturnUrl);
            }
            if (!widget.PageID.IsNullOrEmpty())
            {
                return RedirectToAction("Design", "Page", new { module = "Common", ID = widget.PageID });
            }
            else
            {
                return RedirectToAction("LayoutWidget", "Layout", new { module = "Common" });
            }
        }
        [ViewData_Zones]
        public ActionResult Edit(string ID, string ReturnUrl)
        {
            var widgetService = new WidgetService();
            var widgetBase = widgetService.Get(ID);
            var widget = widgetBase.CreateServiceInstance().GetWidget(widgetBase);
            ViewBag.ReturnUrl = ReturnUrl;
            return View(widget);
        }

        [HttpPost, ViewData_Zones]
        [ValidateInput(false)]
        public ActionResult Edit(WidgetBase widget, string ReturnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(widget);
            }
            widget.CreateServiceInstance().UpdateWidget(widget);
            if (!ReturnUrl.IsNullOrEmpty())
            {
                return Redirect(ReturnUrl);
            }
            if (!widget.PageID.IsNullOrEmpty())
            {
                return RedirectToAction("Design", "Page", new { module = "Common", ID = widget.PageID });
            }
            else
            {
                return RedirectToAction("LayoutWidget", "Layout", new { module = "Common" });
            }
        }
        [HttpPost]
        public JsonResult SaveWidgetPosition(List<WidgetBase> widgets)
        {
            var widgetService = new WidgetService();
            widgets.Each(m =>
            {
                widgetService.Update(m, new Data.DataFilter(new List<string> { "Position" }).Where<WidgetBase>(n => n.ID, OperatorType.Equal, m.ID));
            });
            return Json(true);
        }
        [HttpPost]
        public JsonResult SaveWidgetZone(WidgetBase widget)
        {
            new WidgetService().Update(widget, new Data.DataFilter(new List<string> { "ZoneID", "Position" }).Where<WidgetBase>(n => n.ID, OperatorType.Equal, widget.ID));
            return Json(true);
        }
        [HttpPost]
        public JsonResult DeleteWidget(string ID)
        {
            WidgetService widgetService = new WidgetService();
            WidgetBase widget = widgetService.Get(ID);
            if (widget != null)
            {
                widget.CreateServiceInstance().DeleteWidget(ID);
                return Json(true);
            }
            return Json(false);
        }
        
    }
}