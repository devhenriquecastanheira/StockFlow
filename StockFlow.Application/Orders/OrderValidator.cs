using FluentValidation;
using StockFlow.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockFlow.Application.Orders;

public class OrderValidator : AbstractValidator<Order>
{
    public OrderValidator()
    {
        RuleFor(order => order.CustomerProfileId)
            .GreaterThan(0);

        RuleFor(order => order.CustomerName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(order => order.CustomerEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);

        RuleFor(order => order.DeliveryStreet)
            .MaximumLength(150);

        RuleFor(order => order.DeliveryNumber)
            .MaximumLength(30);

        RuleFor(order => order.DeliveryCity)
            .MaximumLength(100);

        RuleFor(order => order.DeliveryState)
            .MaximumLength(50);
    }
}
