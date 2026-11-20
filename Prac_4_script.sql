--
-- PostgreSQL database dump
--

\restrict 7Hyd6nasSa1hm5tZfGaxjbgNvkU9R31a9rhzbTdw1z8XZwwdq2o306MOCtHWpt2

-- Dumped from database version 18.6
-- Dumped by pg_dump version 18.6

-- Started on 2026-11-20 16:47:12

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 219 (class 1259 OID 25792)
-- Name: comments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.comments (
    id integer NOT NULL,
    ticket_id integer NOT NULL,
    author_id integer NOT NULL,
    text text NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT comments_text_check CHECK ((TRIM(BOTH FROM text) <> ''::text))
);


ALTER TABLE public.comments OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 25804)
-- Name: comments_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.comments_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.comments_id_seq OWNER TO postgres;

--
-- TOC entry 4982 (class 0 OID 0)
-- Dependencies: 220
-- Name: comments_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.comments_id_seq OWNED BY public.comments.id;


--
-- TOC entry 221 (class 1259 OID 25805)
-- Name: employees; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.employees (
    id integer NOT NULL,
    full_name character varying(150) NOT NULL,
    "position" character varying(100) NOT NULL,
    department character varying(100) NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT employees_full_name_check CHECK ((TRIM(BOTH FROM full_name) <> ''::text))
);


ALTER TABLE public.employees OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 25815)
-- Name: employees_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.employees_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.employees_id_seq OWNER TO postgres;

--
-- TOC entry 4983 (class 0 OID 0)
-- Dependencies: 222
-- Name: employees_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.employees_id_seq OWNED BY public.employees.id;


--
-- TOC entry 223 (class 1259 OID 25816)
-- Name: equipment; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.equipment (
    id integer NOT NULL,
    inventory_number character varying(50) NOT NULL,
    name character varying(150) NOT NULL,
    department character varying(100) NOT NULL,
    is_deleted boolean DEFAULT false NOT NULL,
    CONSTRAINT equipment_inventory_number_check CHECK ((TRIM(BOTH FROM inventory_number) <> ''::text)),
    CONSTRAINT equipment_name_check CHECK ((TRIM(BOTH FROM name) <> ''::text))
);


ALTER TABLE public.equipment OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 25827)
-- Name: equipment_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.equipment_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.equipment_id_seq OWNER TO postgres;

--
-- TOC entry 4984 (class 0 OID 0)
-- Dependencies: 224
-- Name: equipment_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.equipment_id_seq OWNED BY public.equipment.id;


--
-- TOC entry 225 (class 1259 OID 25828)
-- Name: ticket_status_history; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ticket_status_history (
    id integer NOT NULL,
    ticket_id integer NOT NULL,
    from_status character varying(20),
    to_status character varying(20) NOT NULL,
    changed_at timestamp with time zone DEFAULT now() NOT NULL,
    changed_by_id integer NOT NULL,
    CONSTRAINT ticket_status_history_from_status_check CHECK (((from_status)::text = ANY (ARRAY[('New'::character varying)::text, ('InProgress'::character varying)::text, ('Resolved'::character varying)::text, ('Closed'::character varying)::text, ('Cancelled'::character varying)::text]))),
    CONSTRAINT ticket_status_history_to_status_check CHECK (((to_status)::text = ANY (ARRAY[('New'::character varying)::text, ('InProgress'::character varying)::text, ('Resolved'::character varying)::text, ('Closed'::character varying)::text, ('Cancelled'::character varying)::text])))
);


ALTER TABLE public.ticket_status_history OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 25839)
-- Name: ticket_status_history_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ticket_status_history_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.ticket_status_history_id_seq OWNER TO postgres;

--
-- TOC entry 4985 (class 0 OID 0)
-- Dependencies: 226
-- Name: ticket_status_history_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ticket_status_history_id_seq OWNED BY public.ticket_status_history.id;


--
-- TOC entry 227 (class 1259 OID 25840)
-- Name: tickets; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tickets (
    id integer NOT NULL,
    author_id integer NOT NULL,
    equipment_id integer NOT NULL,
    assignee_id integer,
    title character varying(200) NOT NULL,
    description text DEFAULT ''::text NOT NULL,
    priority character varying(20) NOT NULL,
    status character varying(20) DEFAULT 'New'::character varying NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    CONSTRAINT tickets_description_check CHECK ((length(description) <= 2000)),
    CONSTRAINT tickets_priority_check CHECK (((priority)::text = ANY (ARRAY[('Low'::character varying)::text, ('Normal'::character varying)::text, ('High'::character varying)::text, ('Critical'::character varying)::text]))),
    CONSTRAINT tickets_status_check CHECK (((status)::text = ANY (ARRAY[('New'::character varying)::text, ('InProgress'::character varying)::text, ('Resolved'::character varying)::text, ('Closed'::character varying)::text, ('Cancelled'::character varying)::text]))),
    CONSTRAINT tickets_title_check CHECK ((TRIM(BOTH FROM title) <> ''::text))
);


ALTER TABLE public.tickets OWNER TO postgres;

--
-- TOC entry 228 (class 1259 OID 25860)
-- Name: tickets_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tickets_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.tickets_id_seq OWNER TO postgres;

--
-- TOC entry 4986 (class 0 OID 0)
-- Dependencies: 228
-- Name: tickets_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.tickets_id_seq OWNED BY public.tickets.id;


--
-- TOC entry 4775 (class 2604 OID 25861)
-- Name: comments id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.comments ALTER COLUMN id SET DEFAULT nextval('public.comments_id_seq'::regclass);


--
-- TOC entry 4777 (class 2604 OID 25862)
-- Name: employees id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees ALTER COLUMN id SET DEFAULT nextval('public.employees_id_seq'::regclass);


--
-- TOC entry 4779 (class 2604 OID 25863)
-- Name: equipment id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipment ALTER COLUMN id SET DEFAULT nextval('public.equipment_id_seq'::regclass);


--
-- TOC entry 4781 (class 2604 OID 25864)
-- Name: ticket_status_history id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ticket_status_history ALTER COLUMN id SET DEFAULT nextval('public.ticket_status_history_id_seq'::regclass);


--
-- TOC entry 4783 (class 2604 OID 25865)
-- Name: tickets id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets ALTER COLUMN id SET DEFAULT nextval('public.tickets_id_seq'::regclass);


--
-- TOC entry 4967 (class 0 OID 25792)
-- Dependencies: 219
-- Data for Name: comments; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.comments (id, ticket_id, author_id, text, created_at) VALUES (1, 2, 1, 'Проверил блок питания, требуется замена конденсаторов.', '2026-11-19 11:35:32.103032+05');
INSERT INTO public.comments (id, ticket_id, author_id, text, created_at) VALUES (2, 42, 1, 'Диагностирую порт uplink на коммутаторе.', '2026-11-19 15:05:32.103032+05');


--
-- TOC entry 4969 (class 0 OID 25805)
-- Dependencies: 221
-- Data for Name: employees; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.employees (id, full_name, "position", department, is_deleted) VALUES (1, 'Иванов Иван Иванович', 'Инженер техподдержки', 'ИТ-отдел', false);
INSERT INTO public.employees (id, full_name, "position", department, is_deleted) VALUES (2, 'Петрова Анна Сергеевна', 'Системный администратор', 'ИТ-отдел', false);
INSERT INTO public.employees (id, full_name, "position", department, is_deleted) VALUES (3, 'Сидоров Алексей Петрович', 'Бухгалтер', 'Бухгалтерия', false);
INSERT INTO public.employees (id, full_name, "position", department, is_deleted) VALUES (4, 'Смирнов Дмитрий Олегович', 'Стажёр', 'ИТ-отдел', true);


--
-- TOC entry 4971 (class 0 OID 25816)
-- Dependencies: 223
-- Data for Name: equipment; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.equipment (id, inventory_number, name, department, is_deleted) VALUES (1, 'INV-001', 'МФУ HP LaserJet Pro M428fdw', 'Бухгалтерия', false);
INSERT INTO public.equipment (id, inventory_number, name, department, is_deleted) VALUES (2, 'INV-002', 'Рабочая станция Dell OptiPlex 7090', 'Отдел кадров', false);
INSERT INTO public.equipment (id, inventory_number, name, department, is_deleted) VALUES (3, 'INV-003', 'Коммутатор Cisco Catalyst 2960', 'Серверная', false);
INSERT INTO public.equipment (id, inventory_number, name, department, is_deleted) VALUES (4, 'INV-004', 'Старый монитор Samsung', 'Склад', true);


--
-- TOC entry 4973 (class 0 OID 25828)
-- Dependencies: 225
-- Data for Name: ticket_status_history; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (1, 1, NULL, 'New', '2026-11-19 13:35:32.103032+05', 3);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (2, 2, NULL, 'New', '2026-11-19 09:35:32.103032+05', 3);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (3, 2, 'New', 'InProgress', '2026-11-19 10:35:32.103032+05', 1);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (4, 3, NULL, 'New', '2026-11-19 03:35:32.103032+05', 2);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (5, 3, 'New', 'InProgress', '2026-11-19 04:35:32.103032+05', 2);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (6, 3, 'InProgress', 'Resolved', '2026-11-19 05:35:32.103032+05', 2);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (7, 4, NULL, 'New', '2026-11-15 15:35:32.103032+05', 3);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (8, 4, 'New', 'InProgress', '2026-11-16 03:35:32.103032+05', 1);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (9, 4, 'InProgress', 'Resolved', '2026-11-16 11:35:32.103032+05', 1);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (10, 4, 'Resolved', 'Closed', '2026-11-16 15:35:32.103032+05', 3);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (11, 42, NULL, 'New', '2026-11-19 13:35:32.103032+05', 3);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (12, 42, 'New', 'InProgress', '2026-11-19 14:35:32.103032+05', 1);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (13, 43, NULL, 'New', '2026-11-20 16:41:43.634328+05', 1);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (14, 43, 'New', 'InProgress', '2026-11-20 16:42:00.053165+05', 1);
INSERT INTO public.ticket_status_history (id, ticket_id, from_status, to_status, changed_at, changed_by_id) VALUES (15, 43, 'InProgress', 'Resolved', '2026-11-20 16:42:06.390131+05', 1);


--
-- TOC entry 4975 (class 0 OID 25840)
-- Dependencies: 227
-- Data for Name: tickets; Type: TABLE DATA; Schema: public; Owner: postgres
--

INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (1, 3, 1, NULL, 'Замятие бумаги в принтере', 'При печати более 5 страниц застревает бумага в лотке 2.', 'Normal', 'New', '2026-11-19 13:35:32.103032+05');
INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (2, 3, 2, 1, 'Не включается компьютер', 'При нажатии кнопки питания мигает индикатор, экран чёрный.', 'High', 'InProgress', '2026-11-19 10:35:32.103032+05');
INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (3, 2, 3, 2, 'Перегрев коммутатора в стойке', 'Температура поднялась выше нормы, почистили кулеры.', 'Critical', 'Resolved', '2026-11-19 05:35:32.103032+05');
INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (4, 3, 1, 1, 'Замена картриджа', 'Закончился тонер в чёрном картридже.', 'Low', 'Closed', '2026-11-16 15:35:32.103032+05');
INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (5, 3, 2, NULL, 'Установка дополнительного ПО', 'Дубликат заявки, уже установлено.', 'Low', 'Cancelled', '2026-11-18 15:35:32.103032+05');
INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (42, 3, 3, 1, 'Сбой основного маршрутизатора', 'Пропал доступ к корпоративной сети у всего этажа.', 'Critical', 'InProgress', '2026-11-19 14:35:32.103032+05');
INSERT INTO public.tickets (id, author_id, equipment_id, assignee_id, title, description, priority, status, created_at) VALUES (43, 1, 3, 1, 'Пример', '', 'Normal', 'Resolved', '2026-11-20 16:41:43.634328+05');


--
-- TOC entry 4987 (class 0 OID 0)
-- Dependencies: 220
-- Name: comments_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.comments_id_seq', 2, true);


--
-- TOC entry 4988 (class 0 OID 0)
-- Dependencies: 222
-- Name: employees_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.employees_id_seq', 4, true);


--
-- TOC entry 4989 (class 0 OID 0)
-- Dependencies: 224
-- Name: equipment_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.equipment_id_seq', 4, true);


--
-- TOC entry 4990 (class 0 OID 0)
-- Dependencies: 226
-- Name: ticket_status_history_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ticket_status_history_id_seq', 15, true);


--
-- TOC entry 4991 (class 0 OID 0)
-- Dependencies: 228
-- Name: tickets_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tickets_id_seq', 43, true);


--
-- TOC entry 4798 (class 2606 OID 25867)
-- Name: comments comments_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.comments
    ADD CONSTRAINT comments_pkey PRIMARY KEY (id);


--
-- TOC entry 4800 (class 2606 OID 25869)
-- Name: employees employees_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.employees
    ADD CONSTRAINT employees_pkey PRIMARY KEY (id);


--
-- TOC entry 4802 (class 2606 OID 25871)
-- Name: equipment equipment_inventory_number_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipment
    ADD CONSTRAINT equipment_inventory_number_key UNIQUE (inventory_number);


--
-- TOC entry 4804 (class 2606 OID 25873)
-- Name: equipment equipment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.equipment
    ADD CONSTRAINT equipment_pkey PRIMARY KEY (id);


--
-- TOC entry 4806 (class 2606 OID 25875)
-- Name: ticket_status_history ticket_status_history_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ticket_status_history
    ADD CONSTRAINT ticket_status_history_pkey PRIMARY KEY (id);


--
-- TOC entry 4812 (class 2606 OID 25877)
-- Name: tickets tickets_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_pkey PRIMARY KEY (id);


--
-- TOC entry 4807 (class 1259 OID 25878)
-- Name: idx_tickets_assignee_id; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_assignee_id ON public.tickets USING btree (assignee_id);


--
-- TOC entry 4808 (class 1259 OID 25879)
-- Name: idx_tickets_created_at; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_created_at ON public.tickets USING btree (created_at);


--
-- TOC entry 4809 (class 1259 OID 25880)
-- Name: idx_tickets_priority; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_priority ON public.tickets USING btree (priority);


--
-- TOC entry 4810 (class 1259 OID 25881)
-- Name: idx_tickets_status; Type: INDEX; Schema: public; Owner: postgres
--

CREATE INDEX idx_tickets_status ON public.tickets USING btree (status);


--
-- TOC entry 4813 (class 2606 OID 25882)
-- Name: comments comments_author_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.comments
    ADD CONSTRAINT comments_author_id_fkey FOREIGN KEY (author_id) REFERENCES public.employees(id) ON DELETE RESTRICT;


--
-- TOC entry 4814 (class 2606 OID 25887)
-- Name: comments comments_ticket_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.comments
    ADD CONSTRAINT comments_ticket_id_fkey FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE CASCADE;


--
-- TOC entry 4815 (class 2606 OID 25892)
-- Name: ticket_status_history ticket_status_history_changed_by_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ticket_status_history
    ADD CONSTRAINT ticket_status_history_changed_by_id_fkey FOREIGN KEY (changed_by_id) REFERENCES public.employees(id) ON DELETE RESTRICT;


--
-- TOC entry 4816 (class 2606 OID 25897)
-- Name: ticket_status_history ticket_status_history_ticket_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ticket_status_history
    ADD CONSTRAINT ticket_status_history_ticket_id_fkey FOREIGN KEY (ticket_id) REFERENCES public.tickets(id) ON DELETE CASCADE;


--
-- TOC entry 4817 (class 2606 OID 25902)
-- Name: tickets tickets_assignee_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_assignee_id_fkey FOREIGN KEY (assignee_id) REFERENCES public.employees(id) ON DELETE SET NULL;


--
-- TOC entry 4818 (class 2606 OID 25907)
-- Name: tickets tickets_author_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_author_id_fkey FOREIGN KEY (author_id) REFERENCES public.employees(id) ON DELETE RESTRICT;


--
-- TOC entry 4819 (class 2606 OID 25912)
-- Name: tickets tickets_equipment_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_equipment_id_fkey FOREIGN KEY (equipment_id) REFERENCES public.equipment(id) ON DELETE RESTRICT;


-- Completed on 2026-11-20 16:47:13

--
-- PostgreSQL database dump complete
--

\unrestrict 7Hyd6nasSa1hm5tZfGaxjbgNvkU9R31a9rhzbTdw1z8XZwwdq2o306MOCtHWpt2

