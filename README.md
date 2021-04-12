# TestTreeWebApi
#C# #ASP.NET Core #Entity Framework

## API
### TreeNodes
- `GET /TreeNodes` - returns all tree nodes
- `GET /TreeNodes/by-name/{name}` - returns tree node with the specified name
- `GET /TreeNodes/{name}/path` - returns path from tree node with the specified name to root
- `PUT /TreeNodes/{name} [body { name: "string"}]` - returns tree node with the updated name
- `PUT /TreeNodes/{name}/update-parent [body { parentName: "string"}]` - move tree node with the specified name to new parent tree node or to root if parent name is null
- `POST /TreeNodes [body { name: "string"}]` - create tree node with the specified name
- `POST /TreeNodes/{name}/add-child [body { name: "string"}]` - add tree node to a tree node with passed name
- `DELETE /TreeNodes/{name}` - delete tree node with the specified name and all its children
