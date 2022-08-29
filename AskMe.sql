create database AskMe;

use AskMe;

create table "User"(
	ID int identity(1,1),
	
	JoiningDatetime datetime not null constraint df_user_joining_datetime default getDate(),
	Name varchar(50) not null,
	Email varchar(100) not null,
	Username varchar(20) not null,
	"Password" varchar(20) not null,
	"Role" nvarchar(20) constraint df_user_role default 'user',
	"Status" bit not null constraint df_user_status default 1,
	QuestionCount int not null constraint df_user_question_count default 0,
	AnswerCount int not null constraint df_user_answer_count default 0,
	ReportCount int not null constraint df_user_report_count default 0,
	
	constraint pk_user_id primary key(ID),
	constraint uq_user_username unique(Username),
	constraint uq_user_email unique(Email),  
	constraint chk_user_email check(Email like '%_@%_.%_'),
	constraint chk_user_username check(len(Username) >= 5 and len(Username) <= 20),
	constraint chk_user_password check(len("Password") >= 5)
);

create table Question(
	ID int identity(1,1),
	
	Title varchar(250) not null,
	"Description" text,
	CreatorID int not null,
	PostingDatetime datetime not null constraint df_question_posting_datetime default getDate(),
	ModificationDatetime datetime not null constraint df_question_modification_datetime default getDate(),
	ViewCount int not null constraint df_question_view_count default 0,
	"Status" bit not null constraint df_question_status default 1,
	
	constraint pk_question_id primary key(ID),
	constraint fk_question_creator_id foreign key(CreatorID) references "User"(ID)
);

create table Answer(
	ID int identity(1,1),
	
	Content text not null,
	CreatorID int not null,
	QuestionID int not null,
	PostingDatetime datetime not null constraint df_answer_posting_datetime default getDate(),
	ModificationDatetime datetime not null constraint df_answer_modification_datetime default getDate(),
	UpvoteCount int not null constraint df_answer_upvote_count default 0,
	DownvoteCount int not null constraint df_answer_downvote_count default 0,
	"Status" bit not null constraint df_answer_status default 1,
	
	constraint pk_answer_id primary key(ID),
	constraint fk_answer_creator_id foreign key(CreatorID) references "User"(ID),
	constraint fk_answer_question_id foreign key(QuestionID) references Question(ID) on delete cascade
);

create table Vote(
	ID int identity(1,1),
	
	AnswerID int not null,
	UserID int not null,
	"Status" bit not null constraint df_vote_status default 1,
	
	constraint pk_vote_id primary key(ID),
	constraint fk_vote_user_id foreign key(UserID) references "User"(ID),
	constraint fk_vote_answer_id foreign key(AnswerID) references Answer(ID) on delete cascade,
	constraint uq_vote_answer_user_id unique(AnswerID, UserID)
);

create table Tag(
	ID int identity(1,1),
	
	Keyword varchar(50) not null,
	
	constraint pk_tag_id primary key(ID),
	constraint uq_tag_keyword unique(Keyword)
);

create table QuestionTag(
	ID int identity(1,1),
	
	QuestionID int not null,
	TagID int not null,
	
	constraint pk_question_tag_id primary key(ID),
	constraint fk_question_tag_question_id foreign key(QuestionID) references Question(ID) on delete cascade,
	constraint fk_question_tag_tag_id foreign key(TagID) references Tag(ID) on delete cascade,
	constraint uq_question_tag unique(QuestionID, TagID)
);

create table QuestionReport(
	ID int identity(1,1),
	
	QuestionID int not null,
	ReporterID int not null,
	ReportHandlerID int,
	Reason varchar(500),
	ReportDatetime datetime not null constraint df_question_report_datetime default getDate(),
	"Status" bit not null constraint df_question_report_status default 1,
	
	constraint pk_question_report_id primary key(ID),
	constraint fk_question_report_question_id foreign key(QuestionID) references Question(ID) on delete cascade,
	constraint fk_question_report_reporter_id foreign key(ReporterID) references "User"(ID),
	constraint fk_question_report_report_handler_id foreign key(ReportHandlerID) references "User"(ID),
	constraint uq_question_report_question_reporter_id unique(QuestionID, ReporterID)
);

create table AnswerReport(
	ID int identity(1,1),
	
	AnswerID int not null,
	ReporterID int not null,
	ReportHandlerID int,
	Reason varchar(500),
	ReportDatetime datetime not null constraint df_answer_report_datetime default getDate(),
	"Status" bit not null constraint df_answer_report_status default 1,
	
	constraint pk_answer_report_id primary key(ID),
	constraint fk_answer_report_answer_id foreign key(AnswerID) references Answer(ID) on delete cascade,
	constraint fk_answer_report_reporter_id foreign key(ReporterID) references "User"(ID),
	constraint fk_answer_report_report_handler_id foreign key(ReportHandlerID) references "User"(ID),
	constraint uq_answer_report_answer_reporter_id unique(AnswerID, ReporterID)
);
