import { Category, CookingDifficulty, ShortRecipe, Tag } from 'types';
import photo from '../assets/img/Photo.png';
import photo2 from '../assets/img/photo_desert.jpg';

export const recipeMock1: ShortRecipe = {
    id: '1',
    title: 'Пироженое тирамису',
    mainPictureUrl: photo,
    cookingTime: 140,
    cookingDifficulty: CookingDifficulty.hard,
    calories: 200,
    tags: [Tag.vegetarian],
    popularity: 10,
    rating: 3.5,
    category: Category.dessert,
};

export const recipeMock2: ShortRecipe = {
    id: '2',
    title: 'Торт с вишней и карамельным сиропом',
    mainPictureUrl: photo2,
    cookingTime: 20,
    cookingDifficulty: CookingDifficulty.medium,
    calories: 300,
    tags: [Tag.lean],
    popularity: 20,
    rating: 9,
    category: Category.dessert,
};

export const recipeMock3: ShortRecipe = {
    id: '3',
    title: 'Пироженое с шоколадом и вишней', 
    mainPictureUrl: photo,
    cookingTime: 40,
    cookingDifficulty: CookingDifficulty.easy,
    calories: 400,
    tags: [Tag.vegetarian, Tag.lean],
    popularity: 30,
    rating: 6.5,
    category: Category.dessert,
};

export const arrayRecipesMock = [recipeMock1, recipeMock2, recipeMock3, recipeMock1, recipeMock2, recipeMock3];
