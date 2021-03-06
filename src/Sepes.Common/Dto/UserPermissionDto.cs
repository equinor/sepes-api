﻿namespace Sepes.Common.Dto
{
    public class UserPermissionDto
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }

        public bool Admin { get; set; }

        public bool Sponsor { get; set; }

        public bool DatasetAdmin { get; set; }



        public bool CanCreateStudy { get; set; }

        public bool CanRead_PreApproved_Datasets { get; set; }

        public bool CanEdit_PreApproved_Datasets { get; set; }

    }
}
