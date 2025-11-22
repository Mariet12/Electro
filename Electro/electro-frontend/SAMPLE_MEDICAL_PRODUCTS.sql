-- ملف SQL لإضافة منتجات طبية نموذجية
-- استخدمي هذا الملف لإضافة المنتجات في قاعدة البيانات

-- ===================================
-- 1. إضافة الفئات الطبية
-- ===================================

INSERT INTO Categories (Name, Description, ImageUrl, CreatedAt, IsDeleted) VALUES
('أجهزة طبية احترافية', 'أجهزة طبية متقدمة للمستشفيات والعيادات', 'http://localhost:5000/uploads/categories/medical-devices.jpg', GETDATE(), 0),
('أجهزة قياس طبية', 'أجهزة قياس الضغط والسكر والحرارة', 'http://localhost:5000/uploads/categories/measurement.jpg', GETDATE(), 0),
('معدات غرف العمليات', 'معدات جراحية وغرف عمليات', 'http://localhost:5000/uploads/categories/operation.jpg', GETDATE(), 0),
('أجهزة الأشعة والتصوير', 'أجهزة MRI, CT, X-Ray', 'http://localhost:5000/uploads/categories/imaging.jpg', GETDATE(), 0),
('أدوات طبية', 'أدوات طبية متنوعة', 'http://localhost:5000/uploads/categories/tools.jpg', GETDATE(), 0);

-- ===================================
-- 2. إضافة المنتجات
-- ===================================

-- المنتج 1: السماعة الطبية
INSERT INTO Products (Name, Description, Price, DiscountedPrice, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'سماعة طبية Littmann Classic III',
    'سماعة طبية احترافية من 3M Littmann، توفر جودة صوت استثنائية للكشف الدقيق على المرضى. مصممة للاستخدام الطبي المهني في العيادات والمستشفيات.
    
المميزات:
• تقنية الصوت المزدوج - يمكن سماع الأصوات عالية ومنخفضة التردد
• جودة صوت فائقة لفحص القلب والرئتين
• مريحة للأذن مع سدادات ناعمة قابلة للتبديل
• أنبوب مقاوم للزيوت والكحول
• ضمان لمدة سنتين من الشركة المصنعة
• متوفرة بألوان متعددة
    
المواصفات التقنية:
• الطول: 69 سم
• الوزن: 150 جرام
• قطر الصدرية: 4.3 سم
• مصنوعة من الفولاذ المقاوم للصدأ',
    2800,
    2500,
    25,
    5, -- أدوات طبية
    'Littmann 3M',
    'الولايات المتحدة',
    GETDATE(),
    0
);

-- إضافة صور المنتج 1
DECLARE @Product1Id INT = SCOPE_IDENTITY();
INSERT INTO ProductImages (ProductId, ImageUrl, CreatedAt) VALUES
(@Product1Id, 'http://localhost:5000/uploads/products/stethoscope-1.jpg', GETDATE()),
(@Product1Id, 'http://localhost:5000/uploads/products/stethoscope-2.jpg', GETDATE());

-- المنتج 2: جهاز التخدير
INSERT INTO Products (Name, Description, Price, DiscountedPrice, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'جهاز التخدير الطبي SUPERSTAR - موديل S6100',
    'جهاز تخدير طبي متطور للعمليات الجراحية في غرف العمليات والعناية المركزة. يوفر تحكم دقيق في التخدير مع نظام تهوية آمن للمريض.
    
المميزات الرئيسية:
• شاشة LCD ملونة عالية الوضوح 15 بوصة
• نظام تهوية ميكانيكي متقدم مع عدة أنماط
• مراقبة مستمرة لجميع المعايير الحيوية
• نظام إنذار ذكي لضمان سلامة المريض
• مناسب للبالغين والأطفال
• سهل التنظيف والتعقيم
• استهلاك منخفض للأكسجين
• بطارية احتياطية تدوم 30 دقيقة
    
المواصفات:
• حجم التنفس: 20-1500 مل
• معدل التنفس: 4-100 نفس/دقيقة
• نسبة الأكسجين: 21-100%
• ضغط التهوية: 0-80 سم ماء
• أبعاد: 110×60×150 سم
• الوزن: 180 كجم
• الطاقة: 220V/50Hz',
    280000,
    265000,
    3,
    1, -- أجهزة طبية احترافية
    'SUPERSTAR Medical',
    'الصين',
    GETDATE(),
    0
);

DECLARE @Product2Id INT = SCOPE_IDENTITY();
INSERT INTO ProductImages (ProductId, ImageUrl, CreatedAt) VALUES
(@Product2Id, 'http://localhost:5000/uploads/products/anesthesia-machine.jpg', GETDATE());

-- المنتج 3: طاولة العمليات الجراحية
INSERT INTO Products (Name, Description, Price, DiscountedPrice, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'طاولة عمليات جراحية كهربائية شاملة - مع وحدة إضاءة',
    'طاولة عمليات جراحية كهربائية متعددة الوظائف مع نظام إضاءة LED جراحي متطور. مصممة لتوفير أعلى معايير الراحة والدقة في العمليات الجراحية.
    
المميزات:
• تحكم كهربائي كامل في جميع الحركات
• سطح طاولة من الفولاذ المقاوم للصدأ
• وسادات قابلة للإزالة والتعقيم
• نظام إضاءة جراحي LED مزدوج بدون ظل
• شدة إضاءة قابلة للتعديل
• ارتفاع قابل للتعديل: 70-110 سم
• إمالة جانبية: ±20 درجة
• إمالة Trendelenburg: ±25 درجة
• حمولة آمنة: 250 كجم
• نظام طوارئ يدوي احتياطي
• عجلات بفرامل مركزية
    
يشمل:
• وحدة إضاءة جراحية LED مزدوجة
• ذراع دعم الوحدة
• مساند جانبية للذراعين
• مسند للساقين قابل للتعديل
• لوحة تحكم لاسلكية
• شاشات جانبية للأشعة
    
المواصفات:
• الأبعاد: 210×50 سم
• الطاقة: 220V AC
• ضمان: 3 سنوات',
    450000,
    425000,
    2,
    3, -- معدات غرف العمليات
    'Medical Systems',
    'ألمانيا',
    GETDATE(),
    0
);

DECLARE @Product3Id INT = SCOPE_IDENTITY();
INSERT INTO ProductImages (ProductId, ImageUrl, CreatedAt) VALUES
(@Product3Id, 'http://localhost:5000/uploads/products/operation-table.jpg', GETDATE());

-- المنتج 4: جهاز MRI
INSERT INTO Products (Name, Description, Price, DiscountedPrice, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'جهاز التصوير بالرنين المغناطيسي MRI - 1.5 Tesla',
    'جهاز تصوير بالرنين المغناطيسي (MRI) عالي الدقة بقوة 1.5 تسلا، يوفر صور تشخيصية واضحة جداً لجميع أجزاء الجسم. مثالي للمستشفيات ومراكز الأشعة المتقدمة.
    
المميزات التقنية:
• قوة مغناطيسية: 1.5 تسلا - توازن مثالي بين الجودة والتكلفة
• فتحة واسعة: 70 سم - مريحة للمرضى
• طول النفق: 150 سم
• دقة صورة فائقة الوضوح
• تقنية تقليل الضوضاء - 99 ديسيبل
• وقت فحص سريع
• برامج فحص متخصصة للدماغ، القلب، العمود الفقري، المفاصل
• نظام تبريد مغلق لا يحتاج صيانة متكررة
    
التطبيقات:
• تصوير الدماغ والحبل الشوكي
• تصوير القلب والأوعية الدموية
• تصوير الأورام
• تصوير المفاصل والعظام
• تصوير الأعضاء الداخلية
    
يشمل:
• وحدة التحكم والمعالجة
• شاشة عرض طبية عالية الدقة
• محطة عمل للطبيب
• نظام الأرشفة PACS
• تدريب شامل للفريق الطبي
• صيانة سنة مجانية
    
المواصفات:
• المساحة المطلوبة: 40 متر مربع
• الطاقة: 3 Phase, 380V
• التبريد: نظام مغلق
• الضمان: 5 سنوات على المغناطيس، سنتين على الأجزاء',
    8500000,
    NULL,
    1,
    4, -- أجهزة الأشعة والتصوير
    'Siemens Healthineers',
    'ألمانيا',
    GETDATE(),
    0
);

DECLARE @Product4Id INT = SCOPE_IDENTITY();
INSERT INTO ProductImages (ProductId, ImageUrl, CreatedAt) VALUES
(@Product4Id, 'http://localhost:5000/uploads/products/mri-scanner.jpg', GETDATE());

-- المنتج 5: جهاز قياس الضغط
INSERT INTO Products (Name, Description, Price, DiscountedPrice, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'جهاز قياس ضغط الدم الرقمي الأوتوماتيكي للذراع',
    'جهاز قياس ضغط الدم الرقمي الأوتوماتيكي، سهل الاستخدام للمنزل أو العيادة. دقة عالية معتمدة من منظمة الصحة العالمية.
    
المميزات:
• قياس أوتوماتيكي بالكامل - ضغط الزر فقط
• شاشة LCD كبيرة وواضحة
• قياس الضغط الانقباضي والانبساطي
• قياس نبضات القلب
• ذاكرة لـ 90 قراءة لشخصين (180 قراءة إجمالي)
• مؤشر منظمة الصحة العالمية (WHO) بالألوان
• كشف عدم انتظام ضربات القلب
• متوسط آخر 3 قراءات تلقائياً
• كفة ذراع قابلة للتعديل: 22-42 سم
• وضع الضيف - لا يحفظ القراءة
• إيقاف تلقائي لتوفير البطارية
    
سهل الاستخدام:
• زر واحد لبدء القياس
• نفخ وتفريغ أوتوماتيكي
• صوت تنبيه عند الانتهاء
• رموز واضحة على الشاشة
    
يشمل:
• الجهاز الرئيسي
• كفة ذراع قياسية
• 4 بطاريات AA
• حقيبة تخزين
• دليل المستخدم بالعربية
• كارت ضمان سنتين
    
المواصفات:
• نطاق القياس: 0-299 ملم زئبق
• الدقة: ±3 ملم زئبق
• نطاق النبض: 40-180 نبضة/دقيقة
• الأبعاد: 13×10×6 سم
• الوزن: 350 جرام (بدون بطاريات)
• معتمد من FDA و CE
• مصنوع حسب المعايير الأوروبية',
    850,
    699,
    50,
    2, -- أجهزة قياس طبية
    'Omron',
    'اليابان',
    GETDATE(),
    0
);

DECLARE @Product5Id INT = SCOPE_IDENTITY();
INSERT INTO ProductImages (ProductId, ImageUrl, CreatedAt) VALUES
(@Product5Id, 'http://localhost:5000/uploads/products/blood-pressure.jpg', GETDATE());

-- ===================================
-- 3. منتجات إضافية مقترحة
-- ===================================

-- جهاز قياس السكر
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'جهاز قياس السكر في الدم Accu-Chek Active',
    'جهاز سويسري دقيق لقياس مستوى الجلوكوز في الدم مع 50 شريط اختبار. نتيجة سريعة في 5 ثوانٍ.',
    650,
    40,
    2,
    'Accu-Chek',
    'سويسرا',
    GETDATE(),
    0
);

-- سرير طبي كهربائي
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'سرير طبي كهربائي 3 حركات مع ريموت كنترول',
    'سرير طبي عالي الجودة بـ 3 موتورات كهربائية، حواجز جانبية، فرشة طبية، حمولة 250 كجم',
    18500,
    8,
    3,
    'Medical Pro',
    'ألمانيا',
    GETDATE(),
    0
);

-- كرسي متحرك
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'كرسي متحرك ألومنيوم قابل للطي - خفيف الوزن',
    'كرسي متحرك من الألومنيوم، خفيف جداً 15 كجم، قابل للطي، عجلات كبيرة، مساند قابلة للإزالة',
    3200,
    15,
    3,
    'Karma',
    'تايوان',
    GETDATE(),
    0
);

-- ترمومتر رقمي
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'ترمومتر جبهة بالأشعة تحت الحمراء - بدون لمس',
    'قياس الحرارة عن بعد بدون لمس، مثالي للأطفال، ذاكرة 32 قراءة، شاشة ملونة، إنذار حمى',
    380,
    60,
    2,
    'Braun',
    'ألمانيا',
    GETDATE(),
    0
);

-- جهاز بخار
INSERT INTO Products (Name, Description, Price, Stock, CategoryId, Brand, CountryOfOrigin, CreatedAt, IsDeleted)
VALUES (
    'جهاز استنشاق البخار الطبي (نيبولايزر)',
    'جهاز بخار كومبريسور لعلاج الربو والحساسية، صامت، سريع، مع ماسكين بالغ وطفل',
    750,
    30,
    2,
    'Omron',
    'اليابان',
    GETDATE(),
    0
);

PRINT 'تم إضافة المنتجات الطبية بنجاح!';
PRINT 'عدد المنتجات المضافة: 10';
PRINT '';
PRINT 'ملاحظة مهمة:';
PRINT 'يرجى نسخ الصور إلى المجلد التالي:';
PRINT 'Electro.Api/wwwroot/uploads/products/';

