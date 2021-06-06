select 
	bs.id, 
	bs.name, 
	'Bookstore' as search_type 
from bookstore bs 
	where 
		@hasBookstoreRead = true and
		(strpos(bs.name, @searchPhrase) > 0) OR (strpos(bs.location, @searchPhrase) > 0) 
	
union all

select 
	b.id, 
	b."name", 
	'Book' as search_type 
from book b 
	where
		@hasBookRead = true and
		(strpos(b.name, @searchPhrase) > 0) OR (strpos(b.author, @searchPhrase) > 0) 

order by name