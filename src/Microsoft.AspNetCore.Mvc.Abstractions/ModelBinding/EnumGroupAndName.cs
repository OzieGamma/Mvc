// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Microsoft.Extensions.Internal;
using Microsoft.Extensions.Localization;

namespace Microsoft.AspNetCore.Mvc.ModelBinding
{
    /// <summary>
    /// An abstraction used when grouping enum values for <see cref="ModelMetadata.EnumGroupedDisplayNamesAndValues"/>.
    /// </summary>
    public struct EnumGroupAndName
    {
        private IStringLocalizer _stringLocalizer;
        private FieldInfo _fieldInfo;
        private string _name;

        /// <summary>
        /// Initializes a new instance of the EnumGroupAndName structure. Should not be used if localization is in use.
        /// </summary>
        /// <param name="group">The group name.</param>
        /// <param name="name">The name.</param>
        public EnumGroupAndName(string group, string name)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Group = group;

            _stringLocalizer = null;
            _fieldInfo = null;
            _name = name;
        }

        /// <summary>
        /// Initializes a new instance of the EnumGroupAndName structure.
        /// </summary>
        /// <param name="group">The group name.</param>
        /// <param name="stringLocalizer">The <see cref="IStringLocalizer"/> to localize with.</param>
        /// <param name="fieldInfo">The <see cref="FieldInfo"/> to use in localization.</param>
        public EnumGroupAndName(
            string group,
            IStringLocalizer stringLocalizer,
            FieldInfo fieldInfo)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (fieldInfo == null)
            {
                throw new ArgumentNullException(nameof(fieldInfo));
            }

            Group = group;
            _stringLocalizer = stringLocalizer;
            _fieldInfo = fieldInfo;
            _name = null;
        }

        /// <summary>
        /// Gets the Group name.
        /// </summary>
        public string Group { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return GetDisplayName();
            }
        }

        // Equals and GetHashCode must be overloaded to accomidate the _nameFunc
        public override bool Equals(object obj)
        {
            if (!(obj is EnumGroupAndName))
            {
                return false;
            }

            var second = (EnumGroupAndName)obj;

            return string.Equals(Group, second.Group) && string.Equals(Name, second.Name);
        }

        public override int GetHashCode()
        {
            var hashcode = HashCodeCombiner.Start();

            hashcode.Add(Group);
            hashcode.Add(Name);

            return hashcode;
        }

        private string GetDisplayName()
        {
            if (_fieldInfo == null)
            {
                return _name;
            }
            else
            {
                var display = _fieldInfo.GetCustomAttribute<DisplayAttribute>(inherit: false);
                if (display != null)
                {
                    // Note [Display(Name = "")] is allowed.
                    var name = display.GetName();
                    if (_stringLocalizer != null && !string.IsNullOrEmpty(name) && display.ResourceType == null)
                    {
                        name = _stringLocalizer[name];
                    }
                    return name ?? _fieldInfo.Name;
                }

                return _fieldInfo.Name;
            }
        }
    }
}
