﻿using BilgeAdam.Data.Abstractions;
using BilgeAdam.Data.Dtos;
using BilgeAdam.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BilgeAdam.Data.Concretes
{
    public class ProductService : IProductService
    {
        private DatabaseManager databaseManager;
       
        public ProductService()
        {
            databaseManager = new DatabaseManager();   
        }

        public bool CreateProduct(NewProductDto productDto)
        {
            var query = @$"INSERT INTO Products (ProductName,
                                                 UnitPrice,
                                                 UnitsInStock,
                                                 CategoryID,
                                                 SupplierID) 
                                        VALUES
                                                ('{productDto.Name}',
                                                  {productDto.UnitPrice},
                                                  {productDto.UnitsInStock},
                                                  {productDto.CategoryID},
                                                  {productDto.SupplierID})";
            return databaseManager.Created(query);
        }

        public bool DeleteProduct(int product)
        {
            var query = $"DELETE FROM Products WHERE ProductID = {product}";
            return databaseManager.DeleteProduct(query);

        }

        public List<Product> GetAllProduct()
        {
            var query = "SELECT * FROM Products";
           return databaseManager.GetAllWithDapper(query);
        }

        public List<ComboBoxItemDto> GetCategories()
        {
            var query = "SELECT CategoryID AS ID, CategoryName AS NAME FROM Categories";
            return databaseManager.GetCategoriesWithDapper(query);
        }

        public void GetFilterData(string query, Action<SqlDataReader> mapperProduct)
        {
            var filterQuery = @$"SELECT p.ProductID AS Id,
                                 p.ProductName AS Name,
                                 p.UnitPrice AS Price,
                                 p.UnitsInStock AS Stock,
                                 c.CategoryName AS Category,
                                 s.CompanyName AS Company
                          FROM Products p
                          INNER JOIN Categories c ON c.CategoryID = p.CategoryID
                          INNER JOIN Suppliers s ON s.SupplierID = p.SupplierID
                          {query}";
            databaseManager.GetOffSetProduct(filterQuery, mapperProduct);
        }

        public void GetProductByOffSet(int pageIndex, int pageSize, Action<SqlDataReader> mapperProduct)
        {
            var query = @$"SELECT p.ProductID AS Id,
                                 p.ProductName AS Name,
                                 p.UnitPrice AS Price,
                                 p.UnitsInStock AS Stock,
                                 c.CategoryName AS Category,
                                 s.CompanyName AS Company
                          FROM Products p
                          INNER JOIN Categories c ON c.CategoryID = p.CategoryID
                          INNER JOIN Suppliers s ON s.SupplierID = p.SupplierID
                          ORDER BY p.ProductID
                          OFFSET {pageIndex * pageSize} ROWS FETCH NEXT {pageSize} ROWS ONLY";
            databaseManager.GetOffSetProduct(query, mapperProduct);

        }

        public List<ComboBoxItemDto> GetSuppliers()
        {
            var query = @"SELECT SupplierID AS ID, CompanyName AS Name FROM Suppliers";
            return databaseManager.GetSuppliersWithDapper(query);
        }

        public int GetTotalCount()
        {
            var query = "SELECT COUNT(0) FROM Products";
            return databaseManager.GetTotalCountWithDapper(query);
        }

        public bool UpdateProduct(ProductDto product, ComboBoxItemDto selectedCategory, ComboBoxItemDto selectedSupplier)
        {
            var query = $@"UPDATE Products SET 
                                           ProductName ='{product.Name}',
                                           UnitPrice ={product.Price},
                                           UnitsInStock ={product.Stock},
                                           CategoryID ={selectedCategory.ID},
                                           SupplierID ={selectedSupplier.ID}
                                  WHERE ProductID ={product.Id}";
            return databaseManager.UpdateProductWithDapper(query);
        }
    }
}
