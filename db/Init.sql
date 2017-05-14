drop schema dbo cascade;

create schema dbo;

-- словари
create table dbo.category (
    id serial primary key,
    value varchar(50) not null
);
insert into dbo.category (id, value) values (1, 'IT'), (2, 'Медицина'), (3, 'Космос'), (4, 'Пузожители'), (5, 'Алкоголь'), (6, 'Другое');

create table dbo.visibility (
    id serial primary key,
    value varchar(50) not null
);
insert into dbo.visibility (id, value) values (1, 'Для всех'), (2, 'Для некоторых'), (3, 'Невидимый');

create table dbo.idea_status (
    id serial primary key,
    value varchar(50) not null
);
insert into dbo.idea_status (id, value) values (1, 'Черновик'), (2, 'Опубликована'), (3, 'Не допущена'), (4, 'Ожидает подтверждения');
-- /словари

create table dbo.user (
    id serial primary key,
    name varchar(255),
    email varchar(255) unique not null,
    login varchar(50) unique not null,
    password varchar(255) not null,
	visibility_id int not null default(1),
    
    constraint fk__user__visibility FOREIGN KEY (visibility_id)
    	REFERENCES dbo.visibility (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table dbo.vendor (
    id serial primary key,
    user_id int unique not null,
    short_description varchar(1000),
    
    constraint fk__vendor__user FOREIGN KEY (user_id)
    	REFERENCES dbo.user (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table dbo.idea (
    id serial primary key,
    owner_id int not null,
    name varchar(50) not null,
    short_description varchar(1000) not null,
    full_description varchar not null,
	rating double precision not null,
	cost double precision,
	status_id int not null default(1),
    
    constraint fk__idea__idea_status FOREIGN KEY (status_id)
    	REFERENCES dbo.idea_status (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table dbo.file (
    id serial primary key,
    name varchar(50) not null,
    content bytea not null,
	content_type varchar(50) not null
);

create table dbo.idea_category (
    idea_id int not null,
    category_id int not null,
    
    constraint fk__idea_category__idea FOREIGN KEY (idea_id)
    	REFERENCES dbo.idea (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION,
    constraint fk__idea_category__category FOREIGN KEY (category_id)
    	REFERENCES dbo.category (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table dbo.vendor_category (
    vendor_id int not null,
    category_id int not null,
    
    constraint fk__vendor_category__vendor FOREIGN KEY (vendor_id)
    	REFERENCES dbo.vendor (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION,
    constraint fk__vendor_category__category FOREIGN KEY (category_id)
    	REFERENCES dbo.category (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

create table dbo.idea_file (
    idea_id int not null,
    file_id int not null,
    
    constraint fk__idea_file__idea FOREIGN KEY (idea_id)
    	REFERENCES dbo.idea (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION,
    constraint fk__idea_file__file FOREIGN KEY (file_id)
    	REFERENCES dbo.file (id) MATCH SIMPLE
    	ON UPDATE NO ACTION ON DELETE NO ACTION
);

-- Test

insert into dbo.user (id, email, login, password) values (1, 'test@test.ts', 'TestUser', 'qwerty');
insert into dbo.vendor (id, user_id, short_description) values (1, 1, 'My short description');
insert into dbo.idea (owner_id, name, short_description, full_description, rating, cost, status_id) values
	(1, 'N', 'S', 'F', 1.11, 2.22, 1),
	(1, 'N', 'S', 'F', -1.11, 2.52, 2),
	(1, 'N', 'S', 'F', 1.11, -9.73, 3),
	(1, 'N', 'S', 'F', 1.11, 2.22, 4);
