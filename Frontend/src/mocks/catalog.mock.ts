import { ShortRecipe } from 'types';
import photo from '../assets/img/Photo.png';
import photo2 from '../assets/img/photo_desert.jpg';

export const recipeMock1: ShortRecipe = {
    id: '1',
    title: 'Пироженое тирамису',
    mainImage: photo,
    cookingTime: 140,
    difficulty: 'easy',
    calories: 200,
    tags: ['Тег 1', 'Тег 2'],
    popularity: 10,
    rating: 3.5,
};

export const recipeMock2: ShortRecipe = {
    id: '2',
    title: 'Торт с вишней и карамельным сиропом',
    mainImage: photo2,
    cookingTime: 20,
    difficulty: 'medium',
    calories: 300,
    tags: ['Тег 1', 'Тег 2'],
    popularity: 20,
    rating: 9,
};

export const recipeMock3: ShortRecipe = {
    id: '3',
    title: 'Пироженое с шоколадом и вишней', 
    mainImage: photo,
    cookingTime: 40,
    difficulty: 'hard',
    calories: 400,
    tags: ['Тег 1', 'Тег 2'],
    popularity: 30,
    rating: 6.5,
};

export const arrayRecipesMock = [recipeMock1, recipeMock2, recipeMock3, recipeMock1, recipeMock2, recipeMock3];
