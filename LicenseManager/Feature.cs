// <copyright file="Feature.cs" company="Charles W. Bozarth">
// Copyright (C) 2009-2012 Charles W. Bozarth
// Refer to License.cs for the full copyright notice.
// </copyright>

// Revisions:
//
// 2009-09-10 CWB
// The regular expressions for finding the feature name, vendor daemon name and user name have been
// changed from matching a word character (\w) to matching non-white space character (\S).
// A license was found where the feature name included a period which is not a word character.
// It was decided that a vendor daemon name and user name could have the same issue and were also updated.

namespace CWBozarth.LicenseManager
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Represents the feature of a license.
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Stores the name.
        /// </summary>
        private string name;

        /// <summary>
        /// Stores the quantity issued.
        /// </summary>
        private int quantityIssued;

        /// <summary>
        /// Stores the users of the feature.
        /// </summary>
        private List<User> users = new List<User>();

        /// <summary>
        /// Stores the vendor.
        /// </summary>
        private string vendor;

        /// <summary>
        /// Stores the version.
        /// </summary>
        private string version;

        /// <summary>
        /// Stores the type.
        /// </summary>
        private string type;

        /// <summary>
        /// Stores error state of the feature.
        /// </summary>
        private bool hasError;

        /// <summary>
        /// Stores the error message.
        /// </summary>
        private string errorMessage;

        /// <summary>
        /// Stores the full feature information.
        /// </summary>
        private string report;

        /// <summary>
        /// Initializes a new instance of the Feature class.
        /// </summary>
        /// <remarks>
        /// Typically a feature is created for a License by the internal constructor. This default constructor is provided
        /// as a means of instantiating an empty feature object outside of the License class.
        /// </remarks>
        public Feature()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Feature class by parsing the feature information read from a license.
        /// </summary>
        /// <remarks>
        /// This is an internal constructor because only the License class is expected to provide the
        /// proper string for initialization.
        /// </remarks>
        /// <param name="featureEntry">The feature's information as read from the lmstat output.</param>
        internal Feature(string featureEntry)
        {
            // Since this class is re-instanced each time the license file is parsed there is no need
            // to ensure all the properties are set for each 'if' statement. The properties will be
            // initialized to their default already.
            this.report = featureEntry;
            
            // Example:
            // Users of Used_Feature_One_License:  (Total of 1 license issued;  Total of 1 licenses in use)
            // Note that quantity in use is not read from this. It is calculated from the number of users found.
            Regex featureExpression = new Regex(@"^Users of (?<name>\S+):\s+\(Total of (?<qty>\d+) \w+ issued[^\r]+", RegexOptions.Multiline);
            Match featureMatch = featureExpression.Match(this.report);
            if (featureMatch.Success)
            {
                this.name = featureMatch.Groups["name"].Value;
                this.quantityIssued = int.Parse(featureMatch.Groups["qty"].Value, CultureInfo.InvariantCulture);

                // Example:
                // Users of Used_Feature_One_License:  (Total of 1 license issued;  Total of 1 licenses in use)
                //
                //   "Used_Feature_One_License" v22.0, vendor: testdaemon
                //   floating license
                Regex detailExpression = new Regex(@"^Users of (?<name>\S+):.*\""\k<name>\"" (?<version>v[0-9.]+), vendor: (?<vendor>\S+)[\r\n\s]*(?<type>[^\r]*)", RegexOptions.Multiline | RegexOptions.Singleline);
                Match detailMatch = detailExpression.Match(this.report);
                if (detailMatch.Success)
                {
                    this.version = detailMatch.Groups["version"].Value;
                    this.vendor = detailMatch.Groups["vendor"].Value;
                    this.type = detailMatch.Groups["type"].Value;
                }

                // Examples:
                // user001 comp001 comp001 (v22.0) (SERVER001/27001 3861), start Tue 3/17 7:13
                // user005 comp005 comp005 (v22.0) (SERVER001/27001 601), start Tue 3/17 10:18 (linger: 14437140)
                Regex userExpression = new Regex(@"\S+ \S+ \S+ \(\S+\) \(\S+/\d+ \d+\), start.+", RegexOptions.Multiline);
                MatchCollection matches = userExpression.Matches(this.report);
                foreach (Match userMatch in matches)
                {
                    // We do not need to clear the collection first because the Feature object is created new
                    // each time the license file is parsed.
                    this.users.Add(new User(userMatch.Value));
                }
            }
            else
            {
                // Example:
                // Users of Feature_With_Other_Error:  (Error: 21 licenses, unsupported by licensed server)
                Regex unsupportedExpression = new Regex(@"^Users of (?<name>\S+):\s+\((?<message>Error: (?<qty>\d+) licenses,[^\r]+)\)", RegexOptions.Multiline);
                Match unsupportedMatch = unsupportedExpression.Match(this.report);
                if (unsupportedMatch.Success)
                {
                    this.name = unsupportedMatch.Groups["name"].Value;
                    this.quantityIssued = int.Parse(unsupportedMatch.Groups["qty"].Value, CultureInfo.InvariantCulture);
                    this.errorMessage = unsupportedMatch.Groups["message"].Value;
                    this.hasError = true;
                }
                else
                {
                    // Example:
                    // Users of Feature_With_Error: Cannot get users of Feature_With_Error: No such feature exists. (-5,222)
                    Regex errorExpression = new Regex(@"^Users of (?<name>\S+):\s+(?<message>[^\r]+)", RegexOptions.Multiline);
                    Match errorMatch = errorExpression.Match(this.report);
                    if (errorMatch.Success)
                    {
                        this.name = errorMatch.Groups["name"].Value;
                        this.errorMessage = errorMatch.Groups["message"].Value;
                        this.hasError = true;
                    }
                }
            }
        }

        /// <summary>
        /// Gets the name of the feature.
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// Gets the quantity of issued licenses.
        /// </summary>
        public int QuantityIssued
        {
            get { return this.quantityIssued; }
        }

        /// <summary>
        /// Gets the quantity of licenses in use.
        /// </summary>
        public int QuantityUsed
        {
            get { return this.users.Count; }
        }

        /// <summary>
        /// Gets the quantity of licenses that are available (not in use).
        /// </summary>
        /// <remarks>This is simply subtracting QuantityUsed from QuantityIssued, but is provided for data binding.</remarks>
        public int QuantityAvailable
        {
            get { return this.QuantityIssued - this.QuantityUsed; }
        }

        /// <summary>
        /// Gets the quantity of licenses that are borrowed.
        /// </summary>
        public int QuantityBorrowed
        {
            get
            {
                int count = 0;

                foreach (User user in this.users)
                {
                    if (user.IsBorrowed)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Gets a list of the users.
        /// </summary>
        public ReadOnlyCollection<User> Users
        {
            get { return new ReadOnlyCollection<User>(this.users); }
        }

        /// <summary>
        /// Gets the vendor.
        /// </summary>
        public string Vendor
        {
            get { return this.vendor; }
        }

        /// <summary>
        /// Gets the version.
        /// </summary>
        public string Version
        {
            get { return this.version; }
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        public string Type
        {
            get { return this.type; }
        }

        /// <summary>
        /// Gets a value indicating whether the feature is being used.
        /// </summary>
        public bool InUse
        {
            get { return this.users.Count != 0; }
        }

        /// <summary>
        /// Gets a value indicating whether the feature has an error.
        /// </summary>
        public bool HasError
        {
            get { return this.hasError; }
        }

        /// <summary>
        /// Gets the error message.
        /// </summary>
        public string ErrorMessage
        {
            get { return this.errorMessage; }
        }

        /// <summary>
        /// Gets the full feature information from lmstat.
        /// </summary>
        public string Report
        {
            get { return this.report; }
        }

        /// <summary>
        /// Gets the name of the feature.
        /// </summary>
        /// <returns>The name of the feature.</returns>
        public override string ToString()
        {
            return this.name;
        }
    }
}
