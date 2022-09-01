using BST.Downstream.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace BST.Downstream.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NetworkController : ControllerBase
    {
        [HttpPost("GetNumberOfDownstreamCustomers")]
        public IActionResult GetNumberOfDownstreamCustomers(RequestObject requestObject)
        {
            BinaryTree binaryTree = new ();
            binaryTree.SelectedNode = requestObject.SelectedNode;
            var list = new List<Branch>();
            foreach (var branch in requestObject.Network.Branches)
            {
                var tempList = new List<Branch>(requestObject.Network.Branches);
                tempList.Remove(branch);
                var startNodeExists = tempList.Where(x => x.StartNode == branch.StartNode || x.EndNode == branch.StartNode).Any();
                var endNodeExists = tempList.Where(x => x.StartNode == branch.EndNode || x.EndNode == branch.EndNode).Any();
                if (startNodeExists || endNodeExists)
                {
                    list.Add(branch);
                }
            }
            foreach (var node in list)
            {
                var existingStartNode = binaryTree.Find(node.StartNode);
                if (existingStartNode == null)
                {
                    var startCustomerObj = requestObject.Network.Customers.Where(x => x.Node == node.StartNode).FirstOrDefault();
                    var startCustomers = startCustomerObj == null ? 0 : startCustomerObj.NumberOfCustomers;
                    binaryTree.Add(node.StartNode,startCustomers);
                }

                var existingEndNode = binaryTree.Find(node.EndNode);
                if (existingEndNode == null)
                {
                    var endCustomerObj = requestObject.Network.Customers.Where(x => x.Node == node.EndNode).FirstOrDefault();
                    var endCustomers = endCustomerObj == null ? 0 : endCustomerObj.NumberOfCustomers;
                    binaryTree.Add(node.EndNode, endCustomers);
                }
            }
            binaryTree.TraversePreOrder(binaryTree.Root);
            return Ok(binaryTree.TotalCustomers);
        }
    }
}
