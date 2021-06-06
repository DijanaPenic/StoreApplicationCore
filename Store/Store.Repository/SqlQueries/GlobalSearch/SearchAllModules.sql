select 
	bs.id, 
	bs.name, 
	'Bookstore' as search_type 
from bookstore bs 
	where 
		@has_bookstore_read_access = true and
		(strpos(bs.name, @search_phrase) > 0) OR (strpos(bs.location, @search_phrase) > 0) 
	
union all

select 
	b.id, 
	b."name", 
	'Book' as search_type 
from book b 
	where
		@has_book_read_access = true and
		(strpos(b.name, @search_phrase) > 0) OR (strpos(b.author, @search_phrase) > 0) 

order by name