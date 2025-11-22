using Electro.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Order
{
    public sealed class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }

    public sealed class SetPaymentStatusDto
    {
        public bool IsPaid { get; set; }
    }

}
