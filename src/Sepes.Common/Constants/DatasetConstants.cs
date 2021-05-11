namespace Sepes.Infrastructure.Constants
{
    public static class DatasetConstants
    {
        public const string STUDY_SPECIFIC_DATASET_DEFAULT_CONTAINER = "files";

        public const string DATASET_RESTRICTION_TEXT_OPEN = "Inbound network traffic from internet will be turned off but can be opened by a Sepes admin, Sponsor, Sponsor rep. or Vendor admin. Inbound network traffic from the Equinor network will also be turned off but can in the same way be opened by Sepes Admin, Sponsor, Sponsor rep or Vendor admin.\r\n\r\n Outbound network traffic to internet will be turned off but can be opened on request from a Sepes admin, Sponsor, Sponsor rep or Vendor admin.";
        public const string DATASET_RESTRICTION_TEXT_INTERNAL = "Inbound network traffic from internet will be turned off and can only be opened by a Sepes admin, Sponsor or Sponsor rep. Inbound network traffic from the Equinor network will also be turned off and can only be opened by Sepes Admin, Sponsor and Sponsor rep. Access to data and resources in Sandbox will be primarely through the Bastion service.\r\n\r\n Outbound network traffic to internet will be turned off and can only be opened on request from a Vendor admin and approved by Sponsor, Sponsor rep or a Sepes admin.";
        public const string DATASET_RESTRICTION_TEXT_RESTRICTED = "Inbound network traffic from internet will be turned off and can only be opened by a Sepes admin. Inbound network traffic from the Equinor network will also be turned off and can only be opened by Sepes Admin, Sponsor and Sponsor rep. Access to data and resources in Sandbox will be primarely through the Bastion service.\r\n\r\n Outbound network traffic to internet will be turned off and can only be opened on request from Sponsor or Sponsor rep and aproved by a Sepes admin.";
    }
}
