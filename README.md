# TestTreeWebApi
#C# #ASP.NET Core #Entity Framework

## API
### TreeNodes
- `GET /TreeNodes` - returns all tree nodes
- `GET /TreeNodes/{id}` - returns tree node with the specified id
- `GET /TreeNodes/by-name/{name}` - returns tree node with the specified name
- `PUT /TreeNodes/{name}/update-name [body { newName: "string"}]` - returns tree node with the updated name
- `POST /TreeNodes [body { name: "string"}]` - create tree node with the specified name
- `POST /TreeNodes/{name}/add-child [body { name: "string"}]` - add tree node to a tree node with passed name
- `DELETE /TreeNodes/{name}` - delete tree node with the specified name and all its children
