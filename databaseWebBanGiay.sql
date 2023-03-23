create database QLBANGIAY
GO
use QLBANGIAY


GO
CREATE TABLE KHACHHANG
(
	MaKH INT IDENTITY(1,1),
	HoTen nVarchar(50) NOT NULL,
	Taikhoan Varchar(50) UNIQUE,
	Matkhau Varchar(50) NOT NULL,
	Email Varchar(100) UNIQUE,
	DiachiKH nVarchar(200),
	DienthoaiKH Varchar(50),	
	Ngaysinh DATETIME
	CONSTRAINT PK_Khachhang PRIMARY KEY(MaKH)
)

GO
Create Table LOAIGIAY
(
	MaLG int Identity(1,1),
	TenLoaiGiay nvarchar(50) NOT NULL,
	CONSTRAINT PK_Loaigiay PRIMARY KEY(MaLG)
)

Create Table HANGGIAY
(
	MaHG int identity(1,1),
	TenHG nvarchar(50) NOT NULL,
	CONSTRAINT PK_Hanggiay PRIMARY KEY(MaHG)
)


Go
CREATE TABLE GIAY
(
	Magiay INT IDENTITY(1,1),
	Tengiay NVARCHAR(100) NOT NULL,
	Giaban Decimal(18,0) CHECK (Giaban>=0),
	Mota NVarchar(Max),
	Anhbia VARCHAR(50),
	Ngaycapnhat DATETIME,
	Soluongton INT,
	MaLG INT,
	MaHG INT,
	Constraint PK_Giay Primary Key(Magiay),
	Constraint FK_Loaigiay Foreign Key(MaLG) References LOAIGIAY(MaLG),
	Constraint FK_Hanggiay Foreign Key(MaHG)References HANGGIAY(MaHG)
)

GO
CREATE TABLE DONDATHANG
(
	MaDonHang INT IDENTITY(1,1),
	Dathanhtoan bit,
	Tinhtranggiaohang  bit,
	Ngaydat Datetime,
	Ngaygiao Datetime,	
	MaKH INT,
	CONSTRAINT FK_Khachhang FOREIGN KEY (MaKH) REFERENCES Khachhang(MaKH),
	CONSTRAINT PK_DonDatHang PRIMARY KEY (MaDonHang)
)

Go
CREATE TABLE CHITIETDONTHANG
(
	MaDonHang INT,
	Magiay INT,
	Soluong Int Check(Soluong>0),
	Dongia Decimal(18,0) Check(Dongia>=0),	
	CONSTRAINT PK_CTDatHang PRIMARY KEY(MaDonHang,Magiay),
	CONSTRAINT FK_Donhang FOREIGN KEY (Madonhang) REFERENCES Dondathang(Madonhang),
	CONSTRAINT FK_Sach FOREIGN KEY (Magiay) REFERENCES Giay(Magiay)	
)




