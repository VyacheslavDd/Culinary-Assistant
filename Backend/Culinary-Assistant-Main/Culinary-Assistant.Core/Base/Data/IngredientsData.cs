﻿using Culinary_Assistant.Core.DTO.Nutrients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Culinary_Assistant.Core.Base.Data
{
	public static class IngredientsData
	{
		public static Dictionary<string, NutrientsData> Data = new()
{
	{ "помидоры", new NutrientsData(18, 0.5, 0.2, 4.3) },
	{ "огурцы", new NutrientsData(14, 0.7, 0.1, 2.9) },
	{ "морковь", new NutrientsData(32, 1.2, 0.1, 6) },
	{ "капуста белокочанная", new NutrientsData(25, 1.7, 0.1, 5.3) },
	{ "картофель", new NutrientsData(82, 2, 0.1, 19.6) },
	{ "лук репчатый", new NutrientsData(43, 1.6, 0.1, 9.4) },
	{ "чеснок", new NutrientsData(104, 6.4, 0.5, 22) },
	{ "баклажаны", new NutrientsData(23, 0.5, 0.1, 5.4) },
	{ "кабачки", new NutrientsData(25, 0.5, 0.2, 5.6) },
	{ "свекла", new NutrientsData(45, 1.6, 0.1, 10.7) },
	{ "болгарский перец", new NutrientsData(27, 1.3, 0.2, 5.3) },
	{ "тыква", new NutrientsData(26, 1, 0.1, 6.5) },
	{ "шпинат", new NutrientsData(23, 2.9, 0.4, 1.4) },
	{ "салат айсберг", new NutrientsData(14, 0.9, 0.1, 3) },
	{ "брокколи", new NutrientsData(34, 2.8, 0.4, 6.6) },
	{ "цветная капуста", new NutrientsData(30, 2.5, 0.3, 5) },
	{ "редис", new NutrientsData(19, 1.2, 0.1, 3.4) },
	{ "зеленый горошек (свежий)", new NutrientsData(81, 5.4, 0.4, 14.5) },
	{ "петрушка (зелень)", new NutrientsData(49, 3.7, 0.4, 7.6) },
	{ "укроп", new NutrientsData(43, 2.5, 0.5, 6.3) },
	{ "зелёный лук", new NutrientsData(20, 1.3, 0.2, 3.2) },
	{ "квашеная капуста", new NutrientsData(19, 1.8, 0.1, 3) },
	{ "хрен (корень)", new NutrientsData(67, 3.2, 0.4, 14.3) },
	{ "щавель", new NutrientsData(22, 1.5, 0.3, 2.9) },
	{ "ревень", new NutrientsData(21, 0.9, 0.2, 4.5) },
	{ "фасоль стручковая (зелёная)", new NutrientsData(31, 1.8, 0.2, 4.5) },
	{ "репа", new NutrientsData(28, 1.5, 0.1, 6.2) },
	{ "брюква", new NutrientsData(37, 1.2, 0.1, 8.1) },
	{ "фенхель (клубень)", new NutrientsData(31, 1.2, 0.2, 7.3) },
	{ "сельдерей (корень)", new NutrientsData(32, 1.3, 0.3, 7.2) },
	{ "сельдерей (стебель)", new NutrientsData(13, 0.7, 0.1, 2.4) },
	{ "яблоки", new NutrientsData(47, 0.5, 0.2, 11.4) },
	{ "бананы", new NutrientsData(89, 1.4, 0.3, 22.3) },
	{ "апельсины", new NutrientsData(43, 0.9, 0.2, 8.1) },
	{ "груши", new NutrientsData(42, 0.4, 0.1, 10.7) },
	{ "клубника", new NutrientsData(41, 1.7, 0.4, 8.1) },
	{ "виноград", new NutrientsData(72, 0.6, 0.2, 17.1) },
	{ "персики", new NutrientsData(45, 0.9, 0.1, 10.5) },
	{ "сливы", new NutrientsData(46, 0.7, 0.2, 9.8) },
	{ "мандарины", new NutrientsData(38, 0.8, 0.2, 8.5) },
	{ "киви", new NutrientsData(48, 1, 0.6, 10.3) },
	{ "черешня", new NutrientsData(52, 1.1, 0.3, 10.6) },
	{ "абрикосы", new NutrientsData(44, 0.9, 0.1, 9) },
	{ "арбуз", new NutrientsData(27, 0.6, 0.2, 6.5) },
	{ "дыня", new NutrientsData(34, 0.6, 0.3, 7.4) },
	{ "малина", new NutrientsData(52, 1.2, 0.7, 11.7) },
	{ "черника", new NutrientsData(57, 1, 0.6, 12.2) },
	{ "гранат", new NutrientsData(83, 1.7, 1, 14.5) },
	{ "ананас", new NutrientsData(50, 0.5, 0.1, 13.1) },
	{ "лимон", new NutrientsData(29, 0.9, 0.3, 9.3) },
	{ "манго", new NutrientsData(60, 0.8, 0.4, 14.9) },
	{ "инжир (свежий)", new NutrientsData(74, 0.8, 0.3, 19.2) },
	{ "хурма", new NutrientsData(67, 0.5, 0.4, 15.3) },
	{ "авокадо", new NutrientsData(160, 2, 14.7, 8.5) },
	{ "финики (свежие)", new NutrientsData(142, 1.8, 0.2, 34) },
	{ "финики (сушёные)", new NutrientsData(282, 2.5, 0.4, 75) },
	{ "изюм", new NutrientsData(299, 2.9, 0.6, 79.2) },
	{ "сушеный инжир", new NutrientsData(257, 3.1, 0.9, 63.9) },
	{ "курага", new NutrientsData(241, 3.4, 0.5, 53.4) },
	{ "чернослив", new NutrientsData(231, 2.3, 0.7, 57.5) },
	{ "куриная грудка", new NutrientsData(165, 20.9, 8.7, 0.5) },
	{ "копчёная куриная грудка", new NutrientsData(184, 23.2, 9.3, 0) },
	{ "куриные бедра (без кожи)", new NutrientsData(184, 19, 11, 0) },
	{ "куриная печень", new NutrientsData(137, 20.4, 5.8, 0.8) },
	{ "сердце куриное", new NutrientsData(159, 15.8, 10.6, 0) },
	{ "говядина", new NutrientsData(187, 18.8, 12.5, 0) },
	{ "фарш говяжий (15% жирности)", new NutrientsData(241, 17.2, 19, 0) },
	{ "свинина (нежирная)", new NutrientsData(263, 16.3, 27.9, 0) },
	{ "бекон", new NutrientsData(510, 23, 45, 0) },
	{ "грудинка свиная копчёная", new NutrientsData(518, 10.5, 53, 0) },
	{ "шпик (сало свиное)", new NutrientsData(797, 2.4, 89, 0) },
	{ "индейка", new NutrientsData(198, 21.6, 12.2, 0.9) },
	{ "утка", new NutrientsData(337, 16.6, 61.4, 0.2) },
	{ "гусь", new NutrientsData(364, 16.1, 33.4, 0.2) },
	{ "баранина", new NutrientsData(209, 15.6, 16.3, 0) },
	{ "кролик", new NutrientsData(183, 21, 11, 0) },
	{ "печень говяжья", new NutrientsData(135, 20.4, 3.1, 3.7) },
	{ "язык говяжий", new NutrientsData(209, 14.2, 16.9, 0.3) },
	{ "форель", new NutrientsData(148, 20.5, 7.2, 0) },
	{ "сельдь", new NutrientsData(248, 17.8, 19.4, 0) },
	{ "тунец", new NutrientsData(144, 23, 4.9, 0) },
	{ "лосось", new NutrientsData(208, 20, 13, 0) },
	{ "креветки", new NutrientsData(83, 18, 0.8, 0) },
	{ "кальмары", new NutrientsData(92, 15.5, 1.4, 2) },
	{ "мидии", new NutrientsData(77, 11.5, 2, 3.3) },
	{ "крабовое мясо", new NutrientsData(69, 16, 0.5, 0) },
	{ "икра красная", new NutrientsData(263, 28.8, 9.8, 0) },
	{ "хек", new NutrientsData(86, 16.6, 2.2, 0) },
	{ "минтай", new NutrientsData(72, 15.9, 0.9, 0) },
	{ "судак", new NutrientsData(84, 18.4, 1.1, 0) },
	{ "окунь речной", new NutrientsData(82, 18.2, 1, 0) },
	{ "треска", new NutrientsData(78, 17.7, 0.7, 0) },
	{ "сайра (консервы)", new NutrientsData(250, 18.3, 20.5, 0) },
	{ "икра минтая", new NutrientsData(132, 20, 6.2, 1) },
	{ "угорь копченый", new NutrientsData(326, 14.7, 28.6, 0) },
	{ "семга слабосоленая", new NutrientsData(200, 20, 13, 0) },
	{ "кета", new NutrientsData(138, 21, 5.9, 0) },
	{ "гречка (ядрица)", new NutrientsData(313, 12.6, 3.3, 62.1) },
	{ "гречка зелёная (необжаренная)", new NutrientsData(329, 12.6, 3.1, 67.6) },
	{ "овёс цельный (зерно)", new NutrientsData(389, 16.9, 6.9, 66.3) },
	{ "ячмень (зерно)", new NutrientsData(354, 10, 2.3, 73.5) },
	{ "просо (зерно)", new NutrientsData(378, 11, 4.2, 73.1) },
	{ "рис белый", new NutrientsData(344, 6.7, 0.7, 78.9) },
	{ "рис бурый (необработанный)", new NutrientsData(362, 7.5, 2.7, 76.2) },
	{ "овсянка (геркулес)", new NutrientsData(366, 11.9, 6.2, 69.3) },
	{ "киноа", new NutrientsData(368, 14.1, 6.1, 64.2) },
	{ "булгур", new NutrientsData(342, 12.3, 1.3, 75.9) },
	{ "кускус", new NutrientsData(376, 12.8, 0.6, 77.4) },
	{ "перловка (ячневая крупа)", new NutrientsData(320, 9.3, 1.1, 66.4) },
	{ "пшено", new NutrientsData(348, 11.5, 3.3, 69.3) },
	{ "манка", new NutrientsData(333, 10.3, 1, 70.6) },
	{ "чечевица красная", new NutrientsData(358, 24, 1.5, 60.1) },
	{ "чечевица зеленая", new NutrientsData(352, 25, 1, 56) },
	{ "горох колотый", new NutrientsData(341, 20.5, 1.2, 53.3) },
	{ "фасоль белая (сухая)", new NutrientsData(333, 21.1, 1.6, 60) },
	{ "чиа", new NutrientsData(486, 16.5, 30.7, 42.1) },
	{ "амарант", new NutrientsData(371, 14.5, 7, 65.3) },
	{ "полба (спельта)", new NutrientsData(338, 15.2, 2.4, 61) },
	{ "кукурузная крупа", new NutrientsData(337, 8.3, 1.2, 71) },
	{ "ржаная крупа", new NutrientsData(338, 11, 2.2, 73) },
	{ "пшеничная крупа", new NutrientsData(342, 10.8, 1.3, 71.7) },
	{ "молоко (1.5%)", new NutrientsData(44, 3, 1.5, 4.8) },
	{ "молоко (2.5%)", new NutrientsData(52, 2.8, 2.5, 4.7) },
	{ "молоко (3.2%)", new NutrientsData(60, 2.8, 3.2, 4.7) },
	{ "кефир (1%)", new NutrientsData(40, 3, 1, 4) },
	{ "кефир (2.5%)", new NutrientsData(53, 2.9, 2.5, 4.1) },
	{ "творог (0%)", new NutrientsData(71, 16.5, 0.3, 1.3) },
	{ "творог (5%)", new NutrientsData(121, 16, 5, 1.8) },
	{ "творог (9%)", new NutrientsData(157, 16.7, 9, 2) },
	{ "сметана (10%)", new NutrientsData(116, 2.8, 10, 2.9) },
	{ "сметана (15%)", new NutrientsData(158, 2.8, 15, 3) },
	{ "йогурт натуральный (1.5%)", new NutrientsData(58, 4.3, 1.5, 6.1) },
	{ "йогурт натуральный (2.5%)", new NutrientsData(68, 4.5, 2.5, 6) },
	{ "сыр твердый (45%)", new NutrientsData(350, 25, 28, 0.5) },
	{ "сыр творожный", new NutrientsData(253, 8, 24, 3) },
	{ "сыр моцарелла (45%)", new NutrientsData(280, 18, 20, 1) },
	{ "масло сливочное (82.5%)", new NutrientsData(748, 0.6, 82.5, 0.9) },
	{ "сливки (10%)", new NutrientsData(118, 2.7, 10, 4) },
	{ "сливки (20%)", new NutrientsData(205, 2.7, 20, 4) },
	{ "маскарпоне", new NutrientsData(453, 4, 47, 4) },
	{ "грецкий орех", new NutrientsData(654, 15.2, 65.2, 13.7) },
	{ "миндаль", new NutrientsData(576, 21.2, 49.4, 21.6) },
	{ "фундук", new NutrientsData(628, 15, 61, 16.7) },
	{ "кешью", new NutrientsData(553, 18.2, 43.8, 30.2) },
	{ "семечки подсолнуха", new NutrientsData(584, 20.8, 52.9, 10.5) },
	{ "тыквенные семечки", new NutrientsData(559, 24.5, 45.8, 13.4) },
	{ "арахис", new NutrientsData(567, 26.3, 45.2, 9.9) },
	{ "кедровые орешки", new NutrientsData(673, 13.7, 68.4, 13.1) },
	{ "бразильский орех", new NutrientsData(656, 14.3, 66.4, 12.3) },
	{ "макасдамия", new NutrientsData(718, 7.9, 75.8, 13.8) },
	{ "фисташки", new NutrientsData(562, 20.2, 45, 27.5) },
	{ "семена льна", new NutrientsData(534, 18.3, 42.2, 28.9) },
	{ "семена кунжута", new NutrientsData(573, 17.7, 49.7, 23.4) },
	{ "семена подорожника (псиллиум)", new NutrientsData(21, 2, 0.6, 4) },
	{ "кокосовая стружка", new NutrientsData(592, 5.6, 65, 6.4) },
	{ "арахисовая паста (натуральная)", new NutrientsData(588, 25, 50, 20) },
	{ "яйцо куриное (1 шт, 50 г)", new NutrientsData(70, 6, 5, 0.5) },
	{ "белок куриного яйца", new NutrientsData(17, 3.6, 0.1, 0.2) },
	{ "желток куриного яйца", new NutrientsData(55, 2.7, 4.5, 0.6) },
	{ "яйцо перепелиное", new NutrientsData(14, 1.2, 1, 0.04) },
	{ "яйцо утиное", new NutrientsData(185, 13, 14, 1.5) },
	{ "яйцо индюшиное", new NutrientsData(171, 13.7, 11.9, 1.2) },
	{ "яйцо страуса (на 100 г)", new NutrientsData(118, 12, 10, 0.7) },
	{ "яичный порошок", new NutrientsData(542, 45, 37, 4) },
	{ "шампиньоны", new NutrientsData(27, 4.3, 1, 0.1) },
	{ "белые грибы", new NutrientsData(34, 4.3, 1, 1.1) },
	{ "лисички", new NutrientsData(19, 1.6, 0.5, 1) },
	{ "вешенки", new NutrientsData(33, 3.3, 0.4, 6.1) },
	{ "опята", new NutrientsData(22, 2.2, 1.2, 0.5) },
	{ "маслята", new NutrientsData(20, 2.3, 1, 1.2) },
	{ "рыжики", new NutrientsData(22, 1.9, 0.8, 1.3) },
	{ "подберезовики", new NutrientsData(22, 2.3, 0.9, 1) },
	{ "сыроежки", new NutrientsData(17, 1.7, 0.7, 0.5) },
	{ "грузди", new NutrientsData(19, 1.8, 0.9, 0.7) },
	{ "маслята маринованные", new NutrientsData(15, 1.5, 0.3, 1.2) },
	{ "шиитаке сушеные", new NutrientsData(296, 23.5, 1, 60) },
	{ "трюфели", new NutrientsData(25, 2, 0.5, 2) },
	{ "морские грибы (грибы вуду)", new NutrientsData(16, 1.3, 0.2, 2) },
	{ "эноки", new NutrientsData(37, 2.7, 0.3, 7.8) },
	{ "хлеб пшеничный", new NutrientsData(266, 7.6, 2.5, 49.2) },
	{ "хлеб ржаной", new NutrientsData(210, 5.7, 1.1, 41) },
	{ "хлеб цельнозерновой", new NutrientsData(247, 8.5, 2.5, 44.5) },
	{ "булочка сдобная", new NutrientsData(297, 7.5, 6.9, 51.1) },
	{ "багет", new NutrientsData(270, 8.6, 2.3, 52) },
	{ "лаваш", new NutrientsData(275, 8.4, 1.2, 55.2) },
	{ "тостовый хлеб", new NutrientsData(245, 8.2, 3.3, 45.6) },
	{ "круассан", new NutrientsData(406, 8, 21, 45) },
	{ "слойка", new NutrientsData(430, 6, 24, 46) },
	{ "хлеб бездрожжевой", new NutrientsData(240, 8, 1.5, 47) },
	{ "хлеб на закваске", new NutrientsData(236, 8.2, 1.7, 45) },
	{ "сухари", new NutrientsData(330, 11, 3, 70) },
	{ "гренки", new NutrientsData(375, 9, 20, 40) },
	{ "пита", new NutrientsData(275, 9, 1.2, 56) },
	{ "маффин", new NutrientsData(400, 5, 20, 50) },
	{ "соевый соус", new NutrientsData(53, 8.1, 0.6, 4.9) },
	{ "кетчуп", new NutrientsData(112, 1.8, 0.3, 23.2) },
	{ "майонез (67%)", new NutrientsData(627, 1.4, 67, 3) },
	{ "горчица", new NutrientsData(66, 4.4, 4, 5.7) },
	{ "песто", new NutrientsData(450, 6, 45, 6) },
	{ "томатная паста", new NutrientsData(82, 4.9, 0.5, 18) },
	{ "чесночный соус", new NutrientsData(320, 1.5, 33, 5) },
	{ "тахини", new NutrientsData(595, 17, 53, 21) },
	{ "хумус", new NutrientsData(237, 7.9, 17.1, 14.3) },
	{ "аджика", new NutrientsData(50, 1.2, 2.1, 6.3) },
	{ "винный уксус", new NutrientsData(19, 0, 0, 0) },
	{ "бальзамический уксус", new NutrientsData(88, 0.5, 0, 17) },
	{ "оливковое масло", new NutrientsData(899, 0, 99.9, 0) },
	{ "чёрный перец", new NutrientsData(251, 10.4, 3.3, 38) },
	{ "паприка", new NutrientsData(282, 14.1, 12.9, 54) }
};
	}
}
