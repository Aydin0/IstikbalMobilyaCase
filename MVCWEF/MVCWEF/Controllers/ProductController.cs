using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using MVCWEF.Models;
using System.IO;

namespace MVCWEF.Controllers
{
    public class ProductController : Controller
    {
        string connectionString = @"Data Source = DESKTOP-A2RTCA6; Initial Catalog = MvcCrudDB; Integrated Security=True";
        [HttpGet]
        public ActionResult Index()
        {
            DataTable dtblProduct = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                SqlDataAdapter sqlDa = new SqlDataAdapter("SELECT * FROM Product",sqlCon);
                sqlDa.Fill(dtblProduct);
            }
            return View(dtblProduct);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View(new ProductModel());
        }

        //
        // POST: /Product/Create
        [HttpPost]
        public ActionResult Create(ProductModel productModel)
        {

            var FileName = "";

            if(productModel.ImageFile!=null && productModel.ImageFile.ContentLength > 0)
            {
                FileName = Guid.NewGuid().ToString()+Path.GetExtension(productModel.ImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/files"), FileName);
                productModel.ImageFile.SaveAs(path);
           
            }


            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "INSERT INTO Product VALUES(@ProductName,@Price,@Count,@FileName)";
                SqlCommand sqlCmd = new SqlCommand(query,sqlCon);
                sqlCmd.Parameters.AddWithValue("@ProductName",productModel.ProductName);
                sqlCmd.Parameters.AddWithValue("@Price", productModel.Price);
                sqlCmd.Parameters.AddWithValue("@Count", productModel.Count);
                sqlCmd.Parameters.AddWithValue("@FileName", FileName);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Product/Edit/5
        public ActionResult Edit(int id)
        {
            ProductModel productModel = new ProductModel();
            DataTable dtblProduct = new DataTable();
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "SELECT * FROM Product Where ProductID = @ProductID";
                SqlDataAdapter sqlDa = new SqlDataAdapter(query,sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("@ProductID",id);
                sqlDa.Fill(dtblProduct);
            }
            if (dtblProduct.Rows.Count == 1)
            {
                productModel.ProductID = Convert.ToInt32(dtblProduct.Rows[0][0].ToString());
                productModel.ProductName = dtblProduct.Rows[0][1].ToString();
                productModel.Price = Convert.ToDecimal(dtblProduct.Rows[0][2].ToString());
                productModel.Count = Convert.ToInt32(dtblProduct.Rows[0][3].ToString());
                return View(productModel);
            }
            else
                return RedirectToAction("Index");
        }

        
        public FileResult Download(string fileName)
        {
           byte[] fileBytes =  System.IO.File.ReadAllBytes(Server.MapPath("~/files/"+fileName));
           return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Pdf, fileName);
        }

        //
        // POST: /Product/Edit/5
        [HttpPost]
        public ActionResult Edit(ProductModel productModel)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "UPDATE Product SET ProductName = @ProductName , Price= @price , Count = @Count WHere ProductID = @ProductID";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@ProductID", productModel.ProductID);
                sqlCmd.Parameters.AddWithValue("@ProductName", productModel.ProductName);
                sqlCmd.Parameters.AddWithValue("@Price", productModel.Price);
                sqlCmd.Parameters.AddWithValue("@Count", productModel.Count);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }

        //
        // GET: /Product/Delete/5
        public ActionResult Delete(int id)
        {
            using (SqlConnection sqlCon = new SqlConnection(connectionString))
            {
                sqlCon.Open();
                string query = "DELETE FROM Product WHere ProductID = @ProductID";
                SqlCommand sqlCmd = new SqlCommand(query, sqlCon);
                sqlCmd.Parameters.AddWithValue("@ProductID", id);
                sqlCmd.ExecuteNonQuery();
            }
            return RedirectToAction("Index");
        }
    }
}
