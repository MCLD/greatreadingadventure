using System;
using System.Collections.Generic;
using System.Text;

namespace GRA.Controllers.ViewModel.Avatar
{
    public class DynamicAvatarJsonModel
    {
        public ICollection<DynamicAvatarLayer> Layers { get; set; }

        public class DynamicAvatarLayer
        {
            public int Id { get; set; }

            public ICollection<int> Items { get; set; }
            public ICollection<DynamicAvatarColor> Colors { get; set; }
        }

        public class DynamicAvatarColor
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}
