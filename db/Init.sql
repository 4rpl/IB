drop schema dbo cascade;

create schema dbo;

-- словари
create table dbo.category (
    id serial primary key,
    value varchar(50) not null
);
-- /словари

create table dbo.user (
    id serial primary key,
    email varchar(50) unique not null,
    login varchar(50) unique not null,
    password varchar(50) not null
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
    full_description varchar not null
);

create table dbo.file (
    id serial primary key,
    name varchar(50) not null,
    content bytea not null
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