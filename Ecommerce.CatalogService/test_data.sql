INSERT INTO Categories (Id, Name, ImageUrl, ParentCategoryId)
VALUES 
    ('elec3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d', 'Electronics', 'https://images.example.com/categories/electronics.jpg', NULL),
    ('cloth3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d', 'Clothing & Fashion', 'https://images.example.com/categories/clothing.jpg', NULL),
    ('homeg3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d', 'Home & Garden', 'https://images.example.com/categories/home.jpg', NULL);

INSERT INTO Categories (Id, Name, ImageUrl, ParentCategoryId)
VALUES 
    ('phone3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d', 'Smartphones', 'https://images.example.com/categories/smartphones.jpg', 'elec3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d'),
    ('laptp3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d', 'Laptops & Computers', 'https://images.example.com/categories/laptops.jpg', 'elec3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d');

INSERT INTO Products (Id, Name, Description, ImageUrl, CategoryId, Price, Amount)
VALUES 
    (
        'headp3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Wireless Headphones Pro',
        'Premium wireless headphones with active noise cancellation, 30-hour battery life, and superior sound quality.',
        'https://images.example.com/products/headphones-pro.jpg',
        'elec3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        299.99,
        150
    ),
    (
        'watch3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Smart Watch Ultra',
        'Advanced smartwatch with fitness tracking, heart rate monitor, GPS, and 7-day battery life.',
        'https://images.example.com/products/smartwatch-ultra.jpg',
        'elec3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        449.99,
        200
    ),
    (
        'came3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        '4K Action Camera',
        'Waterproof 4K action camera with image stabilization, perfect for outdoor adventures.',
        'https://images.example.com/products/action-camera.jpg',
        'elec3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        199.99,
        85
    );

INSERT INTO Products (Id, Name, Description, ImageUrl, CategoryId, Price, Amount)
VALUES 
    (
        'tshir3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Premium Cotton T-Shirt',
        'Soft, breathable 100% organic cotton t-shirt available in multiple colors.',
        'https://images.example.com/products/tshirt.jpg',
        'cloth3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        29.99,
        500
    ),
    (
        'jeans3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Classic Denim Jeans',
        'Comfortable slim-fit denim jeans with stretch fabric for all-day comfort.',
        'https://images.example.com/products/jeans.jpg',
        'cloth3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        79.99,
        300
    ),
    (
        'jackt3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Winter Jacket',
        'Warm, waterproof winter jacket with insulated lining and multiple pockets.',
        'https://images.example.com/products/winter-jacket.jpg',
        'cloth3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        149.99,
        120
    );

INSERT INTO Products (Id, Name, Description, ImageUrl, CategoryId, Price, Amount)
VALUES 
    (
        'bulbs3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Smart LED Light Bulbs (4-Pack)',
        'WiFi-enabled smart LED bulbs with RGB color changing, dimmable, and voice control.',
        'https://images.example.com/products/smart-bulbs.jpg',
        'homeg3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        49.99,
        250
    ),
    (
        'kitch3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Bamboo Kitchen Set',
        'Eco-friendly bamboo kitchen utensil set including spatulas, spoons, and cutting board.',
        'https://images.example.com/products/bamboo-kitchen.jpg',
        'homeg3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        39.99,
        180
    ),
    (
        'plant3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Indoor Plant Collection',
        'Set of 3 low-maintenance indoor plants perfect for home or office decoration.',
        'https://images.example.com/products/plants.jpg',
        'homeg3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        59.99,
        95
    );

INSERT INTO Products (Id, Name, Description, ImageUrl, CategoryId, Price, Amount)
VALUES 
    (
        'galxy3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Galaxy Ultra Pro 5G',
        'Latest flagship smartphone with 6.8" AMOLED display, 108MP camera, and 5000mAh battery.',
        'https://images.example.com/products/galaxy-ultra.jpg',
        'phone3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        1099.99,
        75
    ),
    (
        'iphone3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'iPhone Pro Max',
        'Premium iPhone with A17 chip, ProMotion display, and advanced camera system.',
        'https://images.example.com/products/iphone-pro.jpg',
        'phone3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        1199.99,
        60
    ),
    (
        'budget3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Budget Smartphone X',
        'Affordable smartphone with great battery life, decent camera, and smooth performance.',
        'https://images.example.com/products/budget-phone.jpg',
        'phone3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        299.99,
        400
    );

INSERT INTO Products (Id, Name, Description, ImageUrl, CategoryId, Price, Amount)
VALUES 
    (
        'ultra3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'UltraBook Pro 15',
        'Lightweight laptop with Intel i7, 16GB RAM, 512GB SSD, and 10-hour battery life.',
        'https://images.example.com/products/ultrabook-pro.jpg',
        'laptp3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        1299.99,
        50
    ),
    (
        'gamin3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'Gaming Laptop Elite',
        'High-performance gaming laptop with RTX 4070, 32GB RAM, and RGB keyboard.',
        'https://images.example.com/products/gaming-laptop.jpg',
        'laptp3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        1899.99,
        35
    ),
    (
        'macbk3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        'MacBook Air M3',
        'Apple MacBook Air with M3 chip, 13.6" Liquid Retina display, and all-day battery.',
        'https://images.example.com/products/macbook-air.jpg',
        'laptp3d4-e5f6-4a7b-9c8d-1e2f3a4b5c6d',
        1499.99,
        45
    );