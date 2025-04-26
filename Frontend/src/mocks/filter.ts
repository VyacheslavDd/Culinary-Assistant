import { Category, CookingDifficulty, Tag } from 'types';

const DIFFICULTY = [
    { name: 'Легкие', value: CookingDifficulty.easy },
    { name: 'Средние', value: CookingDifficulty.medium },
    { name: 'Сложные', value: CookingDifficulty.hard },
];
const CATEGORY = [
    { name: 'Завтрак', value: Category.breakfast },
    { name: 'Обед', value: Category.dinner },
    { name: 'Суп', value: Category.soup },
    { name: 'Салат', value: Category.salad },
    { name: 'Десерт', value: Category.dessert },
    { name: 'Напитки', value: Category.drinks },
    { name: 'Горячее', value: Category.hot },
    { name: 'Соус', value: Category.sauce },
];
const TAG = [
    { name: 'Вегетарианские', value: Tag.vegetarian },
    { name: 'Постные', value: Tag.lean },
];

export { DIFFICULTY, CATEGORY, TAG };
