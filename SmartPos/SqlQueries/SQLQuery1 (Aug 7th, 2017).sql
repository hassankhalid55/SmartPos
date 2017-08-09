select product.Sn, product.Pid, Product.Description, Product.Price
, IsNull(stocks.StockedIn-stocks.StockedOut, stocks.StockedIn) as 'Qty' from product join
(select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut
from
(select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn' from StockIn s group by
s.Pid) stockins left join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o group by
o.Pid) stockouts on stockins.ProductId = stockouts.ProductId) stocks on 
product.Pid = stocks.ProductId

select o.Pid as 'ProductId',IsNull(sum(o.StockedOut),0) as 'StockedOut' 
from StockOut o group by
o.Pid

select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut
from
(select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn' from StockIn s group by
s.Pid) stockins join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o group by
o.Pid) stockouts on stockins.ProductId = stockouts.ProductId

select product.Sn, product.Pid, Product.Description, Product.Price
, IsNull(stocks.StockedIn-stocks.StockedOut, stocks.StockedIn) as 'Qty' from product join
(select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut
from
(select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn' from StockIn s group by
s.Pid) stockins left join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o group by
o.Pid) stockouts on stockins.ProductId = stockouts.ProductId) stocks on 
product.Pid = stocks.ProductId

select
isnull((select sum(s.StockedIn) as 'StockedIn' from StockIn s where
Pid = 97 group by
s.Pid),0) -
isnull((select sum(o.StockedOut) as 'StockedIn' from StockOut o where
Pid = 97 group by
o.Pid), 0) as 'result'

select (select IsNull(sum(s.StockedIn),0) as 'StockedIn' from StockIn s where Pid =  97  group by s.Pid) -
(select ISNULL(sum(o.StockedOut), 0) as 'StockedIn' from StockOut o where Pid = 97 group by o.Pid) as query

select sum(Price)/count(Pid) as Price from StockIn stockin group by Pid


select product.Sn, product.Pid, Product.Description, stocks.Price  as Price
, IsNull(stocks.StockedIn-stocks.StockedOut, stocks.StockedIn) as 'Qty' from product join
(select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut, stockins.Price
from (select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn', sum(Price)/count(Pid) as Price from StockIn s group by s.Pid) stockins 
left join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o group by
o.Pid) stockouts on stockins.ProductId = stockouts.ProductId
) stocks on 
product.Pid = stocks.ProductId