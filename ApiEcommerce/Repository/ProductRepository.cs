using System;
using ApiEcommerce.Models;
using ApiEcommerce.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ApiEcommerce.Repository;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _db;

    public ProductRepository(ApplicationDbContext db)
    {
        _db = db;
    }

    public bool BuyProduct(string name, int quantity)
    {
        if (string.IsNullOrWhiteSpace(name) || quantity <= 0)
        {
            return false; // Nombre inválido o cantidad no positiva
        }

        var product = _db.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == name.ToLower().Trim());

        if (product == null || product.Stock < quantity)
        {
            return false; // Producto no encontrado o stock insuficiente
        }

        product.Stock -= quantity;
        _db.Products.Update(product);
        return Save();
    }

    public bool CreateProduct(Product product)
    {
        if (product == null)
        {
            return false; // No se puede crear un producto nulo
        }

        product.CreationDate = DateTime.Now;
        product.UpdatedDate = DateTime.Now;
        _db.Products.Add(product);
        return Save();
    }

    public bool DeleteProduct(Product product)
    {
        if (product == null)
        {
            return false; // No se puede eliminar un producto nulo
        }

        _db.Products.Remove(product);
        return Save();
    }

    public Product? GetProduct(int id)
    {
        if (id <= 0)
        {
            return null; // ID inválido
        }

        return _db.Products
        .Include(p => p.Category) // Incluye la entidad relacionada Category
        .FirstOrDefault(p => p.ProductId == id);
    }

    public ICollection<Product> GetProducts()
    {
        return _db.Products
        .Include(p => p.Category) // Incluye la entidad relacionada Category
        .OrderBy(p => p.Name).ToList();
    }

    public ICollection<Product> GetProductsForCategory(int categoryId)
    {
        if (categoryId <= 0)
        {
            return new List<Product>(); // ID de categoría inválida
        }

        return _db.Products.Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
    }

    public bool ProductExists(int id)
    {
        if (id <= 0)
        {
            return false; // ID inválido
        }

        return _db.Products.Any(p => p.ProductId == id);
    }

    public bool ProductExists(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false; // Nombre inválido
        }
        
        return _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool Save()
    {
        return _db.SaveChanges() >= 0;  // Devuelve true si se guardaron cambios
    }

    public ICollection<Product> SearchProduct(string name)
    {
        IQueryable<Product> query = _db.Products; // Inicia la consulta con todos los productos.

        if (!string.IsNullOrWhiteSpace(name))
        {
            query = query.Where(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        return query.OrderBy(p => p.Name).ToList();
    }

    public bool UpdateProduct(Product product)
    {
        if (product == null)
        {
            return false; // No se puede actualizar un producto nulo
        }

        product.UpdatedDate = DateTime.Now;
        _db.Products.Update(product);
        return Save();
    }
}
