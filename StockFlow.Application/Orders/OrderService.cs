using FluentValidation;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using StockFlow.Application.Stock;
using StockFlow.Domain.Entities;
using StockFlow.Domain.Enums;
using StockFlow.Domain.Interfaces;

namespace StockFlow.Application.Orders;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly ICustomerRepository _customerRepository;
    private readonly IValidator<Order> _orderValidator;
    private readonly IStockService _stockService;

    public OrderService(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        ICustomerRepository customerRepository,
        IValidator<Order> orderValidator,
        IStockService stockService)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _customerRepository = customerRepository;
        _orderValidator = orderValidator;
        _stockService = stockService;
    }

    public async Task<List<OrderDto>> GetOrdersAsync()
    {
        var orders = await _orderRepository.GetAllAsync();

        return orders.Select(order => new OrderDto
        {
            Id = order.Id,
            CustomerProfileId = order.CustomerProfileId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            DeliveryAddressId = order.DeliveryAddressId,
            DeliveryStreet = order.DeliveryStreet,
            DeliveryNumber = order.DeliveryNumber,
            DeliveryCity = order.DeliveryCity,
            DeliveryState = order.DeliveryState,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Invoice = GetInvoiceDto(order),
            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        }).ToList();
    }

    public async Task<OrderDto?> GetOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order == null)
        {
            return null;
        }

        return new OrderDto
        {
            Id = order.Id,
            CustomerProfileId = order.CustomerProfileId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            DeliveryAddressId = order.DeliveryAddressId,
            DeliveryStreet = order.DeliveryStreet,
            DeliveryNumber = order.DeliveryNumber,
            DeliveryCity = order.DeliveryCity,
            DeliveryState = order.DeliveryState,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Invoice = GetInvoiceDto(order),
            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<Order> AddOrderAsync(Order order)
    {
        order.Status = OrderStatus.Pending;
        order.CreatedAt = DateTime.UtcNow;

        await _orderValidator.ValidateAndThrowAsync(order);

        await _orderRepository.AddAsync(order);

        return order;
    }

    public async Task<OrderDto?> CheckoutAsync(int userId, int selectedAddressId)
    {
        var profile = await _customerRepository.GetProfileByUserIdAsync(userId);
        if (profile is null)
        {
            return null;
        }

        var address = profile.Addresses.FirstOrDefault(address => address.Id == selectedAddressId);
        if (address is null)
        {
            throw new InvalidOperationException("Endereço de entrega inválido.");
        }

        var cart = await _cartRepository.GetByCustomerProfileIdAsync(profile.Id);
        if (cart is null || cart.Items.Count == 0)
        {
            throw new InvalidOperationException("O carrinho está vazio.");
        }

        var order = new Order
        {
            CustomerProfileId = profile.Id,
            CustomerProfile = profile,
            CustomerName = profile.User.Name,
            CustomerEmail = profile.User.Email,
            DeliveryAddressId = address.Id,
            DeliveryStreet = address.Street,
            DeliveryNumber = address.Number,
            DeliveryCity = address.City,
            DeliveryState = address.State,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Items = cart.Items.Select(item => new OrderItem
            {
                ProductVariantId = item.ProductVariantId,
                ProductVariant = item.ProductVariant,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };

        await _orderValidator.ValidateAndThrowAsync(order);
        await _orderRepository.CheckoutAsync(order, cart.Items.ToList());

        return await GetOrderAsync(order.Id);
    }

    public async Task<OrderDto?> UpdateOrderAsync(int id, Order order)
    {
        await _orderValidator.ValidateAndThrowAsync(order);
        var existingOrder = await _orderRepository.GetByIdAsync(id);

        if (existingOrder is null)
        {
            return null;
        }

        existingOrder.CustomerProfileId = order.CustomerProfileId;
        existingOrder.CustomerName = order.CustomerName;
        existingOrder.CustomerEmail = order.CustomerEmail;
        existingOrder.DeliveryAddressId = order.DeliveryAddressId;
        existingOrder.DeliveryStreet = order.DeliveryStreet;
        existingOrder.DeliveryNumber = order.DeliveryNumber;
        existingOrder.DeliveryCity = order.DeliveryCity;
        existingOrder.DeliveryState = order.DeliveryState;

        await _orderRepository.UpdateAsync(existingOrder);

        return new OrderDto
        {
            Id = existingOrder.Id,
            CustomerProfileId = existingOrder.CustomerProfileId,
            CustomerName = existingOrder.CustomerName,
            CustomerEmail = existingOrder.CustomerEmail,
            DeliveryAddressId = existingOrder.DeliveryAddressId,
            DeliveryStreet = existingOrder.DeliveryStreet,
            DeliveryNumber = existingOrder.DeliveryNumber,
            DeliveryCity = existingOrder.DeliveryCity,
            DeliveryState = existingOrder.DeliveryState,
            CreatedAt = existingOrder.CreatedAt,
            Status = existingOrder.Status,
            Invoice = GetInvoiceDto(existingOrder),
            Items = existingOrder.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _orderRepository.GetByIdAsync(id);

        if (order is null)
        {
            return false;
        }

        await _orderRepository.DeleteAsync(order);

        return true;
    }

    public async Task<OrderDto?> ChangeOrderStatusAsync(int id, OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order is null)
        {
            return null;
        }
        order.Status = newStatus;
        await _orderRepository.UpdateAsync(order);
        return new OrderDto
        {
            Id = order.Id,
            CustomerProfileId = order.CustomerProfileId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            DeliveryAddressId = order.DeliveryAddressId,
            DeliveryStreet = order.DeliveryStreet,
            DeliveryNumber = order.DeliveryNumber,
            DeliveryCity = order.DeliveryCity,
            DeliveryState = order.DeliveryState,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Invoice = GetInvoiceDto(order),
            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<OrderDto?> AddOrderItemAsync(int orderId, CreateOrderItemDto item)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return null;
        }
        var orderItem = new OrderItem
        {
            OrderId = orderId,
            ProductVariantId = item.ProductVariantId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        };
        order.Items.Add(orderItem);
        await _orderRepository.UpdateAsync(order);

        var updatedOrder = await _orderRepository.GetByIdAsync(orderId);

        if (updatedOrder is null)
        {
            return null;
        }

        return new OrderDto
        {
            Id = updatedOrder.Id,
            CustomerProfileId = updatedOrder.CustomerProfileId,
            CustomerName = updatedOrder.CustomerName,
            CustomerEmail = updatedOrder.CustomerEmail,
            DeliveryAddressId = updatedOrder.DeliveryAddressId,
            DeliveryStreet = updatedOrder.DeliveryStreet,
            DeliveryNumber = updatedOrder.DeliveryNumber,
            DeliveryCity = updatedOrder.DeliveryCity,
            DeliveryState = updatedOrder.DeliveryState,
            CreatedAt = updatedOrder.CreatedAt,
            Status = updatedOrder.Status,
            Invoice = GetInvoiceDto(updatedOrder),
            Items = updatedOrder.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<Order?> RemoveOrderItemAsync(int orderId, int itemId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return null;
        }
        var item = order.Items.FirstOrDefault(i => i.Id == itemId);
        if (item is null)
        {
            return null;
        }
        order.Items.Remove(item);
        await _orderRepository.UpdateAsync(order);
        return order;
    }

    public async Task<OrderDto?> ConfirmOrderAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order is null)
        {
            return null;
        }

        if (order.Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException("Apenas pedidos pendentes podem ser confirmados.");
        }

        if (order.Items.Count <= 0)
        {
            throw new InvalidOperationException("O pedido deve conter pelo menos um item para ser confirmado.");
        }

        if (string.IsNullOrWhiteSpace(order.DeliveryStreet) ||
            string.IsNullOrWhiteSpace(order.DeliveryCity) ||
            string.IsNullOrWhiteSpace(order.DeliveryState))
        {
            throw new InvalidOperationException("O pedido precisa ter endereço de entrega.");
        }

        foreach (var item in order.Items)
        {
            await _stockService.RegisterExitAcrossWarehousesAsync(
                item.ProductVariantId,
                item.Quantity,
                $"Pedido #{order.Id} foi confirmado");
        }

        order.Status = OrderStatus.Confirmed;
        order.Invoice = new Invoice
        {
            OrderId = order.Id,
            Number = "FAT-" + order.Id.ToString("D5"),
            IssuedAt = DateTime.UtcNow,
            TotalAmount = order.Items.Sum(item => item.Quantity * item.UnitPrice)
        };

        await _orderRepository.UpdateAsync(order);
        return new OrderDto
        {
            Id = order.Id,
            CustomerProfileId = order.CustomerProfileId,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            DeliveryAddressId = order.DeliveryAddressId,
            DeliveryStreet = order.DeliveryStreet,
            DeliveryNumber = order.DeliveryNumber,
            DeliveryCity = order.DeliveryCity,
            DeliveryState = order.DeliveryState,
            CreatedAt = order.CreatedAt,
            Status = order.Status,
            Invoice = GetInvoiceDto(order),
            Items = order.Items.Select(item => new OrderItemDto
            {
                Id = item.Id,
                ProductVariantId = item.ProductVariantId,
                ProductName = item.ProductVariant.Product.Name,
                Size = item.ProductVariant.Size,
                Color = item.ProductVariant.Color,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList()
        };
    }

    public async Task<byte[]?> GetInvoicePdfAsync(int orderId)
    {
        var order = await _orderRepository.GetByIdAsync(orderId);

        if (order is null || order.Invoice is null)
        {
            return null;
        }

        return CreateInvoicePdf(order);
    }

    private InvoiceDto? GetInvoiceDto(Order order)
    {
        if (order.Invoice is null)
        {
            return null;
        }

        return new InvoiceDto
        {
            Id = order.Invoice.Id,
            OrderId = order.Invoice.OrderId,
            Number = order.Invoice.Number,
            IssuedAt = order.Invoice.IssuedAt,
            TotalAmount = order.Invoice.TotalAmount
        };
    }

    private byte[] CreateInvoicePdf(Order order)
    {
        var invoice = order.Invoice!;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(text => text.FontSize(11));

                page.Header()
                    .Text("Fatura " + invoice.Number)
                    .Bold()
                    .FontSize(20);

                page.Content().Column(column =>
                {
                    column.Spacing(8);

                    column.Item().Text("Pedido #" + order.Id);
                    column.Item().Text("Cliente: " + order.CustomerName);
                    column.Item().Text("Email: " + order.CustomerEmail);
                    column.Item().Text("Data: " + invoice.IssuedAt.ToString("dd/MM/yyyy HH:mm"));
                    column.Item().Text("Entrega: " + order.DeliveryStreet + ", " + order.DeliveryNumber + " - " + order.DeliveryCity + "/" + order.DeliveryState);
                    column.Item().Text("Itens").Bold();

                    foreach (var item in order.Items)
                    {
                        var productName = item.ProductVariant.Product.Name;
                        var line = productName + " - " +
                            item.ProductVariant.Size + " - " +
                            item.ProductVariant.Color + " - " +
                            item.Quantity + " x " +
                            item.UnitPrice.ToString("C");

                        column.Item().Text(line);
                    }

                    column.Item().Text("Total: " + invoice.TotalAmount.ToString("C")).Bold();
                });
            });
        });

        return document.GeneratePdf();
    }
}
