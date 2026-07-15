using Microsoft.EntityFrameworkCore;
using StockFlow.Domain.Entities;

namespace StockFlow.Infrastructure.Data;

public class StockFlowDbContext : DbContext
{
    public StockFlowDbContext(DbContextOptions<StockFlowDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductVariant> ProductVariants => Set<ProductVariant>();
    public DbSet<ProductImage> ProductImages => Set<ProductImage>();
    public DbSet<ProductTag> ProductTags => Set<ProductTag>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<StockItem> StockItems => Set<StockItem>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<PhysicalInventory> PhysicalInventories => Set<PhysicalInventory>();
    public DbSet<PhysicalInventoryItem> PhysicalInventoryItems => Set<PhysicalInventoryItem>();
    public DbSet<User> Users => Set<User>();
    public DbSet<CustomerProfile> CustomerProfiles => Set<CustomerProfile>();
    public DbSet<CustomerAddress> CustomerAddresses => Set<CustomerAddress>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductTag>()
            .HasKey(productTag => new { productTag.ProductId, productTag.TagId });

        modelBuilder.Entity<User>()
            .HasIndex(user => user.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasOne(user => user.CustomerProfile)
            .WithOne(profile => profile.User)
            .HasForeignKey<CustomerProfile>(profile => profile.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomerProfile>()
            .HasMany(profile => profile.Addresses)
            .WithOne(address => address.CustomerProfile)
            .HasForeignKey(address => address.CustomerProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CustomerProfile>()
            .HasMany(profile => profile.Orders)
            .WithOne(order => order.CustomerProfile)
            .HasForeignKey(order => order.CustomerProfileId);

        modelBuilder.Entity<Order>()
            .HasOne<CustomerAddress>()
            .WithMany()
            .HasForeignKey(order => order.DeliveryAddressId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<CustomerProfile>()
            .HasMany(profile => profile.Carts)
            .WithOne(cart => cart.CustomerProfile)
            .HasForeignKey(cart => cart.CustomerProfileId);

        modelBuilder.Entity<Cart>()
            .HasMany(cart => cart.Items)
            .WithOne(item => item.Cart)
            .HasForeignKey(item => item.CartId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CartItem>()
            .HasOne(item => item.ProductVariant)
            .WithMany()
            .HasForeignKey(item => item.ProductVariantId);

        modelBuilder.Entity<Invoice>()
            .HasOne(invoice => invoice.Order)
            .WithOne(order => order.Invoice)
            .HasForeignKey<Invoice>(invoice => invoice.OrderId);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StockFlowDbContext).Assembly);
    }
}
