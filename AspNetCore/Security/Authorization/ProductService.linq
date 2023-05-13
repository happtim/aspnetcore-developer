<Query Kind="Statements">
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

public interface IProductService
{
	Task<IEnumerable<Product>> GetAll();
	Task<Product> FindAsync(int? id);
	Task<Product> InsertAsync(Product product);
}

public class ProductService : IProductService
{
	//静态产品数组
	private static List<Product> _products = new List<Product>
	{
	};

	public async Task<Product> FindAsync(int? id)
	{
		return await Task.Run(() => _products.FirstOrDefault(u => u.Id == id));
	}

	public async Task<IEnumerable<Product>> GetAll()
	{
		return await Task.Run(() => _products);
	}

	public async Task<Product> InsertAsync(Product product)
	{
		if (product.Id == 0) 
		{
			product.Id = _products.Count()+1;
		}
		_products.Add(product);
		return await Task.FromResult( product);
	}
}

public class Product
{
	public int Id { get; set; }
	public string Name { get; set; }

	public string CreatedUserID { get; set; }
}