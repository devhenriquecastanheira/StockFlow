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
        RuleFor(order => order.CustomerName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(order => order.CustomerEmail)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(150);
    }
}
