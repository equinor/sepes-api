using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sepes.RestApi.Services
{
    public interface IAzureService
    {
        ///<summary>
        ///Creates a resource group with the name provided and returns a resource group ID
        ///</summary>
        ///<param name="resourceGroupName">Name of the pod to create a Resource Group for</param>
        Task<string> CreateResourceGroup(string podName);
      
        ///<summary>
        ///Delete a Resource Group from Azure. WARNING this will also delete all resources contained within that Pod.
        ///</summary>
        ///<param name="resourceGroupName">Name of the Resource Group to delete.</param>
        Task DeleteResourceGroup(string resourceGroupName);

        ///<summary>
        ///Creates a new network within a common resource group where all security groups and networks are stored.
        ///</summary>
        ///<param name="networkName">Name the network to create.</param>
        ///<param name="addressSpace">Specifies the addresspace and mask.</param>
        ///<param name="subnetName">Name of the default subnet to create.</param>
        Task<string> CreateNetwork(string networkName, string addressSpace, string subnetName);
        
        ///<summary>
        ///Delete a Network resource from Azure.
        ///</summary>
        ///<param name="vNetName">Name of the Network to delete.</param>
        Task DeleteNetwork(string vNetName);
      
        ///<summary>
        ///Create a new Network Security Group resource on Azure and add the Base Rules so that Azure load balancer does not interfere with the Pods. This group is created in the common resource group defined in the config
        ///</summary>
        ///<param name="securityGroupName">Unique name of the Network Security Group to create.</param>
        Task CreateSecurityGroup(string securityGroupName);

        ///<summary>
        ///Delete a Network Security Group from Azure. It is required that the NSG is not tied to any subnet before performing this task.
        ///</summary>
        ///<param name="securityGroupName">Name of the Network Security Group to delete.</param>
        Task DeleteSecurityGroup(string securityGroupName);

        ///<summary>
        ///Assign an existing Network Security Group to a subnet
        ///</summary>
        ///<param name="securityGroupName">Name of the Network Security Group to add.</param>
        ///<param name="subnetName">Name of the subnet to modify</param>
        ///<param name="networkName">Name of the network the subnet belongs to.</param>
        Task ApplySecurityGroup(string securityGroupName, string subnetName, string networkName);

        ///<summary>
        ///Remove the association of all Network Security Groups from a subnet.
        ///</summary>
        ///<param name="subnetName">Name of the subnet to modify</param>
        ///<param name="networkName">Name of the network the subnet belongs to.</param>
        Task RemoveSecurityGroup(string subnetName, string networkName);

        ///<summary>
        ///Adds rules to an Nsg. This lets an array of external addresses and a specified port talk to any internal address and port combo.
        ///</summary>
        ///<param name="securityGroupname">Name of the Network Security Group to modify</param>
        ///<param name="ruleName">Gives the rules a name. Must be unique</param>
        ///<param name="priority">A number between 100 and 4096 deciding what rules takes priority over others. Lower numbers have higher priority.</param>
        ///<param name="externalAddresses">An array of external addresses. Written as standard ###.###.###.### as strings.</param>
        ///<param name="toPort">Traffic destination port</param>
        Task NsgAllowOutboundPort(string securityGroupName, string ruleName, int priority, string[] internalAddresses, int internalPort);
        
        ///<summary>
        ///Adds rules to an Nsg. This lets an array of internal addresses and a specified external port talk to any external address and internal port.
        ///</summary>
        ///<param name="securityGroupname">Name of the Network Security Group to modify</param>
        ///<param name="ruleName">Gives the rules a name. Must be unique</param>
        ///<param name="priority">A number between 100 and 4096 deciding what rules takes priority over others. Lower numbers have higher priority.</param>
        ///<param name="internalAddresses">An array of internal addresses. Written as standard ###.###.###.### as strings.</param>
        ///<param name="toPort">Traffic destination port</param>
        Task NsgAllowInboundPort(string securityGroupName, string ruleName, int priority, string[] externalAddresses, int externalPort);
      
        ///<summary>
        ///Gives a user contributor role to a resource group
        ///</summary>
        ///<returns>The users ID</returns>
        ///<param name="userId">A GUID string unique to the user</param>
        ///<param name="resourceGroupName">The name of the resource group to add the user to</param>
        Task<string> AddUserToResourceGroup(string userId, string resourceGroupName);
      
        ///<summary>
        ///Gives a user network join on a network
        ///</summary>
        ///<param name="userId">A GUID string unique to the user</param>
        ///<param name="networkName">The name of the network to add the user to</param>
        Task<string> AddUserToNetwork(string userId, string networkName);
      
        ///<summary>
        ///Gets a string representing all the existing Network Security Groups in the applications common Resource Group as defined in config.
        ///</summary>
        Task<IEnumerable<string>> GetNSGNames();
    }
}
