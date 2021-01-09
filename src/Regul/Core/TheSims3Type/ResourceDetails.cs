using Regul.S3PI.Extensions;
using Regul.S3PI.Interfaces;
using Regul.ViewModels.Controls.ContentTab;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Regul.Core.TheSims3Type
{
    public class ResourceDetails : TheSims3TypeContentViewModel, IResourceKey, IEqualityComparer<IResourceKey>, IEquatable<IResourceKey>, IComparable<IResourceKey>
    {
        private bool _internalchg;
        private TGIN _details;

        public ResourceDetails(bool useName, bool displayFilename)
        {

        }

        public ResourceDetails(bool useName, bool displayFilename, IResourceKey rk)
        {

        }

        public uint ResourceType
        {
            get => this.ResourceTypeView;
            set
            {
                ResourceTypeView = value;
            }
        }
        public uint ResourceGroup { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public ulong Instance { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public int CompareTo(IResourceKey other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IResourceKey x, IResourceKey y)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IResourceKey other)
        {
            throw new NotImplementedException();
        }

        public int GetHashCode([DisallowNull] IResourceKey obj)
        {
            throw new NotImplementedException();
        }

        private void UpdateTGIN()
        {
            if (_internalchg) return;
            _details = new TGIN();
            _details.ResType = ResourceTypeView;

        }
    }
}
