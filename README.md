# TestTreeWebApi
#C# #ASP.NET Core #Entity Framework

## API
### TreeNodes
- `GET /TreeNodes` - returns all TreeNodes
- `GET /TreeNodes/{id}` - returns TreeNode with the specified id
- `GET /TreeNodes/by-name/{name}` - returns returns TreeNode with the specified name
- `PUT /TreeNodes/{name}/update-name [body { newName: "string"}]` - returns TreeNode with the updated name
- `POST /TreeNodes [body { name: "string"}]` - create TreeNode with the specified name
- `POST /TreeNodes/{name}/add-child [body { name: "string"}]` - returns added TreeNode
- `DELETE /TreeNodes/{name}` - delete TreeNode with the specified name and all its childs
