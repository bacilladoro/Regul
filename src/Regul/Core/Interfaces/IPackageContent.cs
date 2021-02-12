using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regul.Core.Interfaces
{
    public interface IPackageContent
    {
        IPackageType PackageType { get; set; }
    }
}
