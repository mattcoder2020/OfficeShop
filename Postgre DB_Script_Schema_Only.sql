-- CartItems
CREATE TABLE public."CartItems" (
    "CartItemId" SERIAL PRIMARY KEY,
    "CartId" INT NOT NULL,
    "ProductId" INT NOT NULL
);

-- Carts
CREATE TABLE public."Carts" (
    "CartId" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "Ordered" VARCHAR(10) NOT NULL,
    "OrderedOn" TEXT NOT NULL
);

-- Offers
CREATE TABLE public."Offers" (
    "OfferId" SERIAL PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Discount" INT NOT NULL
);

-- Orders
CREATE TABLE public."Orders" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "CartId" INT NOT NULL,
    "PaymentId" INT NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

-- PaymentMethods
CREATE TABLE public."PaymentMethods" (
    "PaymentMethodId" SERIAL PRIMARY KEY,
    "Type" TEXT,
    "Provider" TEXT,
    "Available" VARCHAR(50),
    "Reason" TEXT
);

-- Payments
CREATE TABLE public."Payments" (
    "Id" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "PaymentMethodId" INT NOT NULL,
    "TotalAmount" INT NOT NULL,
    "ShippingCharges" INT NOT NULL,
    "AmountReduced" INT NOT NULL,
    "AmountPaid" INT NOT NULL,
    "CreatedAt" TEXT NOT NULL
);

-- ProductCategories
CREATE TABLE public."ProductCategories" (
    "CategoryId" SERIAL PRIMARY KEY,
    "Category" VARCHAR(50) NOT NULL,
    "SubCategory" VARCHAR(50) NOT NULL
);

-- Products
CREATE TABLE public."Products" (
    "ProductId" SERIAL PRIMARY KEY,
    "Title" TEXT NOT NULL,
    "Description" TEXT NOT NULL,
    "CategoryId" INT NOT NULL,
    "OfferId" INT NOT NULL,
    "Price" FLOAT NOT NULL,
    "Quantity" INT NOT NULL,
    "ImageName" TEXT NOT NULL
);

-- Reviews
CREATE TABLE public."Reviews" (
    "ReviewId" SERIAL PRIMARY KEY,
    "UserId" INT NOT NULL,
    "ProductId" INT NOT NULL,
    "Review" TEXT NOT NULL,
    "CreatedAt" VARCHAR(100) NOT NULL
);

-- Users
CREATE TABLE public."Users" (
    "UserId" SERIAL PRIMARY KEY,
    "FirstName" VARCHAR(50) NOT NULL,
    "LastName" VARCHAR(50) NOT NULL,
    "Email" VARCHAR(100) NOT NULL,
    "Address" VARCHAR(100) NOT NULL,
    "Mobile" VARCHAR(15) NOT NULL,
    "Password" VARCHAR(50) NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "ModifiedAt" TEXT NOT NULL
);

-- Foreign Key Constraints
ALTER TABLE public."Orders" ADD CONSTRAINT "FK_Orders_Carts" FOREIGN KEY ("CartId") REFERENCES public."Carts" ("CartId");
ALTER TABLE public."Orders" ADD CONSTRAINT "FK_Orders_Payments" FOREIGN KEY ("PaymentId") REFERENCES public."Payments" ("Id");
ALTER TABLE public."Orders" ADD CONSTRAINT "FK_Orders_Users" FOREIGN KEY ("UserId") REFERENCES public."Users" ("UserId");
ALTER TABLE public."Payments" ADD CONSTRAINT "FK_Payments_PaymentMethods" FOREIGN KEY ("PaymentMethodId") REFERENCES public."PaymentMethods" ("PaymentMethodId");
ALTER TABLE public."Payments" ADD CONSTRAINT "FK_Payments_Users" FOREIGN KEY ("UserId") REFERENCES public."Users" ("UserId");
ALTER TABLE public."Products" ADD CONSTRAINT "FK_Product_Offers" FOREIGN KEY ("OfferId") REFERENCES public."Offers" ("OfferId");
ALTER TABLE public."Products" ADD CONSTRAINT "FK_Product_ProductCategories" FOREIGN KEY ("CategoryId") REFERENCES public."ProductCategories" ("CategoryId");
ALTER TABLE public."Reviews" ADD CONSTRAINT "FK_Reviews_Users" FOREIGN KEY ("UserId") REFERENCES public."Users" ("UserId");