INSERT INTO Products (Id, Sku, InternalCode, Name, Description, CurrentUnitPrice, StockQuantity, IsActive)
VALUES
-- PRODUCTOS ACTIVOS (Visibles)
(NEWID(), 'TEC-MON-001', 'INT-1001', 'Monitor LED 24 Pulgadas', 'Monitor Full HD 1080p ideal para oficina y gaming básico.', 250000.00, 15, 1),
(NEWID(), 'TEC-PER-002', 'INT-1002', 'Teclado Mecánico RGB', 'Teclado gamer con switches azules y luces personalizables.', 85000.00, 30, 1),
(NEWID(), 'TEC-PER-003', 'INT-1003', 'Mouse Inalámbrico Ergo', 'Mouse ergonómico vertical para evitar fatiga en la muñeca.', 45000.00, 50, 1),
(NEWID(), 'AUD-AUR-004', 'INT-2001', 'Auriculares Bluetooth Sony', 'Cancelación de ruido activa y 30 horas de batería.', 180000.00, 10, 1),
(NEWID(), 'MUE-SIL-005', 'INT-3001', 'Silla de Oficina Mesh', 'Silla respirable con soporte lumbar ajustable.', 120000.00, 8, 1),
(NEWID(), 'MUE-ESC-006', 'INT-3002', 'Escritorio Gamer en L', 'Escritorio amplio con soporte para vasos y auriculares.', 210000.00, 5, 1),
(NEWID(), 'COC-CAF-007', 'INT-4001', 'Cafetera Express Automática', 'Prepara café de grano recién molido al instante.', 450000.00, 12, 1),
(NEWID(), 'COC-TOST-008', 'INT-4002', 'Tostadora Eléctrica', 'Tostadora de acero inoxidable para 4 rebanadas.', 60000.00, 25, 1),

-- PRODUCTOS INHABILITADOS (No deberían verse por el cliente, solo por Admin si filtra)
(NEWID(), 'OLD-HDD-001', 'INT-9001', 'Disco Rígido 500GB (Outlet)', 'Disco mecánico refabricado. Garantía limitada.', 25000.00, 2, 0),
(NEWID(), 'OLD-IMP-002', 'INT-9002', 'Impresora Matriz de Puntos', 'Modelo antiguo descontinuado. Solo para repuestos.', 15000.00, 0, 0);