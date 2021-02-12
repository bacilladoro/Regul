using Regul.S3PI.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regul.Core.Interfaces
{
    public interface IPackageType
    {
        string PackageType { get; }

        public void SavePackage(); 
    }
}
