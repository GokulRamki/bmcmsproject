using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using bmcmsproject.Repository;
using bmcmsproject.Models;
using bmcmsproject.Utility;
using System.IO;
using MvcPaging;
using System.Configuration;
using Newtonsoft.Json;
using System.Drawing;
using System.Data.Entity;

namespace bmcmsproject.Controllers
{
    public class AdminController : Controller
    {
        
        private int defaultPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["pgSize"]);
        private int slider_width;
        private int slider_height;
        private UnitOfWork _UOW;
        private IUtilRepository _util_repo;
        public AdminController()
        {
            this._UOW = new UnitOfWork();
            this.slider_width = Convert.ToInt32(ConfigurationManager.AppSettings["slider_width"]);
            this.slider_height = Convert.ToInt32(ConfigurationManager.AppSettings["slider_height"]);
            this._util_repo = new UtilRepository();
        }
        #region index
        public ActionResult Index()
        {
            return RedirectToAction("Login");
        }
        #endregion

        #region Admin Login

        public ActionResult Login()
        {
            try
            {

            }
            catch (Exception )
            {
                
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(adminmodel _obj)
        {
            try
            {
                
                    web_tbl_care_user be_admin = new web_tbl_care_user();
                    //be_admin = _UOW.care_user_repo.Get(f => f.user_name == _obj.username && f.user_pwd == _obj.password && be_admin.is_active == true && be_admin.is_deleted == false).FirstOrDefault();

                    foreach(web_tbl_care_user i in _UOW.care_user_repo.Get().ToList())
                    {
                        if(i.user_name == _obj.username && i.user_pwd == _obj.password )
                        {
                            be_admin = i;
                            Session["user"] = _obj.username;
                            return RedirectToAction("Slider");
                        }
                        else
                    {
                        ViewBag.Msg = "Invalid Username/Password";
                    }
                    }


                   // if (be_admin != null  )
                   // {
                   //     Session["user"] = _obj.username;
                  //      return RedirectToAction("Slider");
                  //  }
                  //  else
                  //{
                  //  ViewBag.Msg = "Invalid Username/Password";
                  //}
                
            }
            catch
            {
                
            }
            return View(_obj);
        }

        public ActionResult Logout()
        {
            try
            {
                if (Session["user"] != null)
                {
                    Session.Clear();
                }
                return RedirectToAction("login");
            }
            catch (Exception )
            {
                
            }
            return View();
        }
        #endregion

        #region slider

        [HttpGet]

        public ActionResult Slider(string slider_name, int? page)
        {
            if (Session["user"] == null)
                return RedirectToAction("login");

            IList<bm_slider_cms> slid_obj = new List<bm_slider_cms>();

            try
            {
                ViewData["slider_name"] = slider_name;
                int currentPageIndex = page.HasValue ? page.Value : 1;

                slid_obj = _UOW.bm_slider_repo.Get(p => p.Is_Deleted == false, orderBy: s => s.OrderBy(p => p.Img_Order)).ToList();

                if (!string.IsNullOrWhiteSpace(slider_name))
                    slid_obj = slid_obj.Where(p => p.Slider_name.ToLower().Contains(slider_name.ToLower())).ToList();

                slid_obj = slid_obj.ToPagedList(currentPageIndex, defaultPageSize);

                if (Request.IsAjaxRequest())
                    return PartialView("_AjaxSlider", slid_obj);
                else
                    return View(slid_obj);
            }
            catch (Exception )
            {
                
            }
            return View(slid_obj);
        }

        [HttpGet]
        public ActionResult bm_slider(long? id)
        { 

            if (Session["user"] == null)
                return RedirectToAction("login");

            long s_id = id.HasValue ? id.Value : 0;
            bm_slider_cms s_obj = new bm_slider_cms();
            try
            {

                if (s_id > 0)
                    s_obj = _UOW.bm_slider_repo.GetByID(s_id);

                return View(s_obj);
            }
            catch (Exception )
            {
                
            }
            return View(s_obj);
        }

        [HttpPost]
        public ActionResult bm_slider(bm_slider_cms _obj, HttpPostedFileBase slid_img)
        {
            if (Session["user"] == null)
                return RedirectToAction("login");

            bm_slider_cms s_obj = new bm_slider_cms();
            try
            {
                if (slid_img == null && _obj.Slider_Img == null)
                {
                    ViewBag.ErMsg = "Please upload the image";
                    return RedirectToAction("bm_slider", "Admin", new { id = 0 });
                }

                if (ModelState.IsValid)
                {
                    s_obj = _obj;
                    Image slider_image = null;
                    string filename = "";
                    string filePath = "";


                    if (slid_img != null)
                    {
                        string Extension = slid_img.FileName.Remove(0, slid_img.FileName.LastIndexOf('.'));
                        if (Extension != "")
                        {
                            if (Extension.ToLower() == ".jpg" || Extension.ToLower() == ".jpeg" || Extension.ToLower() == ".png" || Extension.ToLower() == ".gif")
                            {
                                slider_image = _util_repo.ResizeImage(slid_img.InputStream, slider_width, slider_height);
                                filename = Guid.NewGuid().ToString() + Extension.ToLower();
                                filePath = Server.MapPath("~/Upload/") + filename;

                                s_obj.Slider_Img = filename;
                            }
                            else
                            {
                                ViewBag.ErMsg = "Only jpg,png,gif file formats are allowed to upload..";
                                return View(_obj);
                            }
                        }
                    }

                   

                    if (s_obj.Id == 0)
                    {
                        //int unit = _UOW.bm_slider_repo.Get().ToList().Count()+1;
                        //s_obj.Id = unit;
                        s_obj.Created_On = DateTime.Now;
                        _UOW.bm_slider_repo.Insert(s_obj);
                         
                    }
                    else
                    {
                        s_obj.Modified_On = DateTime.Now;
                        _UOW.bm_slider_repo.Update(s_obj);
                    }

                    if (slider_image != null)
                        slider_image.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
                    _UOW.Save();
                    TempData["success_msg"] = "Slider details has been inserted/updated Successfully";
                    return RedirectToAction("slider", "Admin");

                }
            }
            catch (Exception e )
            {
                
                ViewBag.Msg = "Falied to insert/update in Slider  Details";

            }
            return View(s_obj);
        }

        public ActionResult Delete_Slider(long id)
        {
            try
            {
                if (id > 0)
                {
                    bm_slider_cms _obj = new bm_slider_cms();
                    _obj = _UOW.bm_slider_repo.GetByID(id);
                    if (_obj != null)
                    {
                        bm_slider_cms s_obj = new bm_slider_cms();
                        s_obj = _obj;
                        s_obj.Is_Active = false;
                        s_obj.Is_Deleted = true;
                        _UOW.bm_slider_repo.Update(s_obj);
                        _UOW.Save();

                        string fileUrl = "/Upload/" + _obj.Slider_Img;
                        bool flag = DeleteFile(fileUrl);
                        return Json(new { Status = "true" });
                    }
                }
            }
            catch (Exception )
            {
                
                return Json(new { Status = "false" });
            }
            return null;
        }
        #endregion

        #region FolderCreation & Deletion
        private bool CreateFolderIfNeeded(string path)
        {
            bool result = true;
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception)
                {
                    /*TODO: You must process this exception.*/
                    result = false;
                }
            }
            return result;
        }
        private bool DeleteFile(string path)
        {
            bool result = true;

            string filePath = Path.Combine(Server.MapPath("~" + path));

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
                result = true;
            }

            return result;
        }

#endregion


    }
}