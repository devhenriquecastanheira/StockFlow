using FluentValidation;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Products;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IValidator<Product> _productValidator;

    public ProductService(
        IProductRepository productRepository,
        ITagRepository tagRepository,
        IValidator<Product> productValidator)
    {
        _productRepository = productRepository;
        _tagRepository = tagRepository;
        _productValidator = productValidator;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _productRepository.GetAllAsync();
    }

    public async Task<List<Product>> GetByTagIdAsync(int tagId)
    {
        return await _productRepository.GetByTagIdAsync(tagId);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _productRepository.GetByIdAsync(id);
    }

    public async Task<Product> CreateAsync(Product product)
    {
        await _productValidator.ValidateAndThrowAsync(product);
        await _productRepository.AddAsync(product);

        return product;
    }

    public async Task<Product?> UpdateAsync(int id, Product product)
    {
        await _productValidator.ValidateAndThrowAsync(product);

        var existingProduct = await _productRepository.GetByIdAsync(id);

        if (existingProduct is null)
        {
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.CostPrice = product.CostPrice;
        existingProduct.SalePrice = product.SalePrice;
        existingProduct.CategoryId = product.CategoryId;

        await _productRepository.UpdateAsync(existingProduct);

        return existingProduct;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return false;
        }

        await _productRepository.DeleteAsync(product);

        return true;
    }

    public async Task<List<ProductVariantDto>> GetVariantsAsync()
    {
        var variants = await _productRepository.GetVariantsAsync();
        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            ProductName = v.Product.Name,
            Size = v.Size,
            Color = v.Color,
            Sku = v.Sku,
            MinimumStockLevel = v.MinimumStockLevel
        }).ToList();
    }

    public async Task<ProductVariantDto?> GetVariantByIdAsync(int id)
    {
        var variant = await _productRepository.GetVariantByIdAsync(id);
        if (variant is null)
        {
            return null;
        }

        return new ProductVariantDto
        {
            Id = variant.Id,
            ProductId = variant.ProductId,
            ProductName = variant.Product.Name,
            Size = variant.Size,
            Color = variant.Color,
            Sku = variant.Sku,
            MinimumStockLevel = variant.MinimumStockLevel
        };
    }

    public async Task<List<ProductVariantDto>> GetVariantsByProductIdAsync(int productId)
    {
        var variants = await _productRepository.GetVariantsByProductIdAsync(productId);

        return variants.Select(v => new ProductVariantDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            ProductName = v.Product.Name,
            Size = v.Size,
            Color = v.Color,
            Sku = v.Sku,
            MinimumStockLevel = v.MinimumStockLevel
        }).ToList();
    }

    public async Task<ProductVariantDto> CreateVariantAsync(int productId, ProductVariantDto variant)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        variant.ProductId = productId;

        await _productRepository.AddVariantAsync(new ProductVariant
        {
            ProductId = variant.ProductId,
            Size = variant.Size,
            Color = variant.Color,
            Sku = variant.Sku,
            MinimumStockLevel = variant.MinimumStockLevel
        });

        return variant;
    }

    public async Task<ProductVariantDto?> UpdateVariantAsync(int id, ProductVariantDto variant)
    {
        var existingVariant = await _productRepository.GetVariantByIdAsync(id);

        if (existingVariant is null)
        {
            return null;
        }

        existingVariant.Size = variant.Size;
        existingVariant.Color = variant.Color;
        existingVariant.Sku = variant.Sku;
        existingVariant.MinimumStockLevel = variant.MinimumStockLevel;

        await _productRepository.UpdateVariantAsync(existingVariant);

        return variant;
    }

    public async Task<bool> DeleteVariantAsync(int id)
    {
        var variant = await _productRepository.GetVariantByIdAsync(id);

        if (variant is null)
        {
            return false;
        }

        await _productRepository.DeleteVariantAsync(variant);

        return true;
    }

    public async Task<List<ProductImage>?> GetImagesAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            return null;
        }

        return await _productRepository.GetImagesByProductIdAsync(productId);
    }

    public async Task<ProductImage?> AddImageAsync(int productId, ProductImage image)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product is null)
        {
            return null;
        }

        var productImages = await _productRepository.GetImagesByProductIdAsync(productId);

        if (!productImages.Any())
        {
            image.IsMain = true;
        }

        image.ProductId = productId;
        await _productRepository.AddImageAsync(image);

        if (image.IsMain)
        {
            await _productRepository.SetMainImageAsync(productId, image.Id);
        }

        return image;
    }

    public async Task<ProductImage?> SetMainImageAsync(int productId, int imageId)
    {
        var image = await _productRepository.GetImageByIdAsync(productId, imageId);
        if (image is null)
        {
            return null;
        }

        await _productRepository.SetMainImageAsync(productId, imageId);

        image.IsMain = true;
        return image;
    }

    public async Task<ProductImage?> DeleteImageAsync(int productId, int imageId)
    {
        var image = await _productRepository.GetImageByIdAsync(productId, imageId);
        if (image is null)
        {
            return null;
        }

        var wasMain = image.IsMain;

        await _productRepository.DeleteImageAsync(image);

        if (wasMain)
        {
            var remainingImages = await _productRepository.GetImagesByProductIdAsync(productId);
            var nextMainImage = remainingImages.FirstOrDefault();

            if (nextMainImage is not null)
            {
                await _productRepository.SetMainImageAsync(productId, nextMainImage.Id);
            }
        }

        return image;
    }

    public async Task<List<Tag>?> GetTagsByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
        {
            return null;
        }

        return await _productRepository.GetTagsByProductIdAsync(productId);
    }

    public async Task<Tag?> AddTagAsync(int productId, int tagId)
    {
        var product = await _productRepository.GetByIdAsync(productId);

        if (product is null)
        {
            return null;
        }

        var tag = await _tagRepository.GetByIdAsync(tagId);

        if (tag is null)
        {
            return null;
        }

        var existingProductTag = await _productRepository.GetProductTagAsync(productId, tagId);

        if (existingProductTag is null)
        {
            await _productRepository.AddProductTagAsync(new ProductTag
            {
                ProductId = productId,
                TagId = tagId
            });
        }

        return new Tag
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }

    public async Task<bool> RemoveTagAsync(int productId, int tagId)
    {
        var productTag = await _productRepository.GetProductTagAsync(productId, tagId);

        if (productTag is null)
        {
            return false;
        }

        await _productRepository.DeleteProductTagAsync(productTag);

        return true;
    }
}
