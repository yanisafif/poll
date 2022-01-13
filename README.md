# poll
APPLICATION ASP.NET Core MVC

Ce projet a été développé dans le cadre de nos études à la CCI CAMPUS de strasbourg par Yanis AFIF & Nathan WURTZ.
Le site est déployé ici : https://pollpolo.ddns.net/

Notre projet était de développer un site de sondages en ligne. Avec la technologie ASP.NET CORE MVC.
Vous pouvez cliquer ici pour avoir un rappel détaillé du cahier des charges : 


Pour faire fonctionner ce projet en local : 

- Vous devez utiliser Mysql & VisualStudio

- Vous devez modifier dans le dossier Poll un appsetting.json

- Changer la "ConnectionStrings" & MysqlVersion :

```json
"ConnectionStrings": {
  "MariaDbConnectionString": "server=localhost;database=Poll;user=root"
},
"MysqlVersion": "x.x.x"
```
    
- Appliquer la migration en shell :
```sh
  cd Poll
  
  dotnet ef database update
```
  




