using Electro.Core.Dtos.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Electro.Core.Dtos.Favorite
{
    public class FavoriteDto
    {
        public int Id { get; set; }
        public ProductDto Product { get; set; }
        public DateTime AddedAt { get; set; }
    }

}
