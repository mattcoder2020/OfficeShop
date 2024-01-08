# 办公用品在线商店
### 介绍:

欢迎使用办公用品在线商店应用，这是一个设计用于传统办公用品在线购物平台提供方便的浏览、特价、下单，评价功能，包括办公文具、家具、设备等商品，支持在线支付和订单跟踪服务。

- **演示网站:** [办公用品在线网络演示](http://119.23.216.60:5002/)
- **测试用户凭证:**
  - 用户名: user@user.com
  - 密码: Pa$$w0rd

### 技术堆栈:

**后端:**
- ASP.NET Core 6.0 WebAPI
- PostgreSQL数据库，通过Docker实现可伸缩性
- Ado.net和 Ngsql 用于数据库访问技术
- 响应压缩(Response Compression) 以优化网络流量 
- Swagger提供全面的API文档 
- IdentityModel.Tokens用于JWT令牌的创建、身份验证和基于角色的授权 
- Automapper用于简化对象到对象的映射 
- ASP.NET Core内置的依赖注入 分页、排序和过滤以增强数据处理

**前端:**
- Angular 14框架
- Bootstrap 5.2实现响应式和视觉上吸引人的用户界面
- ngx组件提供额外功能
- PurgeCss用于CSS文件裁剪和优化的
- 路由和安全路由（Routing and secured routes）以增强用户体验
- 根据领域上下文模块化组件和基于路由的懒加载(lazy loading) 集成ngxdatatable实现流畅的表格导航
