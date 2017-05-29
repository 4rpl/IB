drop schema public cascade;

create schema public;

-- словари
create table public.category (
    id serial primary key,
    value varchar(50) not null
);
insert into public.category (id, value) values (1, 'IT'), (2, 'Медицина'), (3, 'Космос'), (4, 'Пузожители'), (5, 'Алкоголь'), (6, 'Другое');

create table public.visibility (
    id serial primary key,
    value varchar(50) not null
);
insert into public.visibility (id, value) values (1, 'Для всех'), (2, 'Для некоторых'), (3, 'Невидимый');

create table public.idea_status (
    id serial primary key,
    value varchar(50) not null
);
insert into public.idea_status (id, value) values (1, 'Черновик'), (2, 'Опубликована'), (3, 'Не допущена'), (4, 'Ожидает подтверждения');
-- /словари

create table public.user (
    id serial primary key,
    name varchar(255),
    email varchar(255) unique not null,
    login varchar(50) unique not null,
    password varchar(255) not null,
	visibility_id int not null default(1),
    
    constraint fk__user__visibility FOREIGN KEY (visibility_id)
    	REFERENCES public.visibility (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table public.vendor (
    id serial primary key,
    user_id int unique not null,
    short_description varchar(1000),
    
    constraint fk__vendor__user FOREIGN KEY (user_id)
    	REFERENCES public.user (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table public.idea (
    id serial primary key,
    owner_id int not null,
    name varchar(50) not null,
    short_description varchar(1000) not null,
    full_description varchar not null,
	rating double precision not null,
	cost double precision,
	status_id int not null default(1),
    
    constraint fk__idea__idea_status FOREIGN KEY (status_id)
    	REFERENCES public.idea_status (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table public.file (
    id serial primary key,
    name varchar(50) not null,
    content bytea not null,
	content_type varchar(50) not null
);

create table public.idea_category (
    idea_id int not null,
    category_id int not null,
    
    constraint fk__idea_category__idea FOREIGN KEY (idea_id)
    	REFERENCES public.idea (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION,
    constraint fk__idea_category__category FOREIGN KEY (category_id)
    	REFERENCES public.category (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table public.vendor_category (
    vendor_id int not null,
    category_id int not null,
    
    constraint fk__vendor_category__vendor FOREIGN KEY (vendor_id)
    	REFERENCES public.vendor (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION,
    constraint fk__vendor_category__category FOREIGN KEY (category_id)
    	REFERENCES public.category (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table public.idea_file (
    idea_id int not null,
    file_id int not null,
    
    constraint fk__idea_file__idea FOREIGN KEY (idea_id)
    	REFERENCES public.idea (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION,
    constraint fk__idea_file__file FOREIGN KEY (file_id)
    	REFERENCES public.file (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Test

insert into public.user (email, login, password) values ('test@test.ts', 'TestUser', 'qwerty');
insert into public.vendor (user_id, short_description) values (1, 'My short description');
insert into public.idea (owner_id, name, short_description, full_description, rating, cost, status_id) values
	(1, 'N', 'S', 'F', 1.11, 2.22, 1),
	(1, 'N', 'S', 'F', -1.11, 2.52, 2),
	(1, 'N', 'S', 'F', 1.11, -9.73, 3),
	(1, 'N', 'S', 'F', 1.11, 2.22, 4);
