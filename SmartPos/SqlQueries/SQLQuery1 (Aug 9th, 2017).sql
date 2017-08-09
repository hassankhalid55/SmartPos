select product.Sn, product.Pid, Product.Description, stocks.Price as Price
, IsNull(stocks.StockedIn-stocks.StockedOut, stocks.StockedIn) as 'Qty' from product join
(select stockins.ProductId, stockins.Stockedin, stockouts.StockedOut, stockins.Price
from (select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn', sum(Price)/count(Pid) as Price from StockIn s group by s.Pid) stockins 
left join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o group by
o.Pid) stockouts on stockins.ProductId = stockouts.ProductId
) stocks on 
product.Pid = stocks.ProductId


-- GETS QUANTITY OF PRODUCTS ALONG WITH THIER PID'S (NOT DESCRIPTIONS)
select stockins.ProductId, (stockins.StockedIn - stockouts.StockedOut) as Qty
from
(select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn' 
from StockIn s 
group by s.Pid) stockins
 join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o 
group by o.Pid) stockouts
on stockins.ProductId = stockouts.ProductId

-- THIS ONE RETURNS PRODUCT DESCRIPTION ALSO
select Product.Description, quantities.Qty
from Product
join
(select stockins.ProductId, (stockins.StockedIn - stockouts.StockedOut) as Qty
from
(select s.Pid as 'ProductId', sum(s.StockedIn) as 'StockedIn' 
from StockIn s 
group by s.Pid) stockins
 join
(select o.Pid as 'ProductId', sum(o.StockedOut) as 'StockedOut' 
from StockOut o 
group by o.Pid) stockouts
on stockins.ProductId = stockouts.ProductId) quantities
on Product.Pid = quantities.ProductId