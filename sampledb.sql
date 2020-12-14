create database sampledb;
use sampledb;
create table users (id bigint not null auto_increment, first_name varchar(20), last_name varchar(20), constraint pk_users primary key (id));
