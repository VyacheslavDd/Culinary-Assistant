import styles from './main-info.module.scss';
import star from '../../../assets/svg/star.svg';
import clock from '../../../assets/svg/clock_pink.svg';
import chef from '../../../assets/svg/chef_pink.svg';
import edit from '../../../assets/svg/edit.svg';
import { useState } from 'react';
import { Modal } from 'components/common';
import { Recipe } from 'types';
import {
    transformCategory,
    transformCookingTime,
    transformDifficulty,
    transformRating,
} from 'utils/transform';
import { useSelector } from 'store/store';
import { selectUser } from 'store/user.slice';
import { useNavigate } from 'react-router';

type props = {
    recipe: Recipe;
};

export function MainInfo(props: props) {
    const { recipe } = props;
    const {
        title,
        description,
        calories,
        cookingTime,
        proteins,
        fats,
        carbohydrates,
        category,
        mainPictureUrl,
        rating,
        cookingDifficulty,
        user,
    } = recipe;
    const [isModalOpen, setIsModalOpen] = useState(false);
    const userId = useSelector(selectUser);
    const navigate = useNavigate();

    const openModal = () => setIsModalOpen(true);
    const closeModal = () => setIsModalOpen(false);

    return (
        <div className={styles.mainContainer}>
            <button
                onClick={openModal}
                className={styles.imageButton}
                aria-label='Увеличить изображение рецепта'
            >
                <img
                    src={mainPictureUrl}
                    alt='recipe'
                    className={styles.image}
                />
            </button>
            <div className={styles.container}>
                <div className={styles.title}>
                    <div className={styles.category}>
                        <p className={styles.tag}>
                            {transformCategory(category)}
                        </p>
                        {userId?.id === user.id ? (
                            <button
                                className={styles.button}
                                onClick={() => {
                                    navigate(`/recipe/${recipe.id}/edit`);
                                }}
                            >
                                <img src={edit} alt='edit' />
                                Редактировать
                            </button>
                        ) : (
                            <></>
                        )}
                    </div>

                    <h2 className={styles.name}>{title}</h2>
                </div>
                <div className={styles.descriptionContainer}>
                    <div className={styles.infoContainer}>
                        <div className={styles.info}>
                            <p className={styles.title}>Рейтинг</p>
                            <img
                                src={star}
                                alt='star'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>
                                {transformRating(rating)}
                            </p>
                        </div>
                        <div className={styles.info}>
                            <p className={styles.title}>Время приготовления</p>
                            <img
                                src={clock}
                                alt='clock'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>
                                {transformCookingTime(cookingTime)}
                            </p>
                        </div>
                        <div className={styles.info}>
                            <p className={styles.title}>Сложность</p>
                            <img
                                src={chef}
                                alt='chef'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>
                                {transformDifficulty(cookingDifficulty)}
                            </p>
                        </div>
                    </div>
                    <p className={styles.description}>{description}</p>
                    <div className={styles.kbju}>
                        <p className={styles.title}>КБЖУ</p>
                        <p>
                            На 100 г:{' '}
                            <span className={styles.text}>
                                {calories} / {proteins} / {fats} /{' '}
                                {carbohydrates}
                            </span>
                        </p>
                    </div>
                </div>
            </div>
            {isModalOpen && (
                <Modal onClose={closeModal}>
                    <img
                        src={mainPictureUrl}
                        alt='Увеличенное изображение рецепта'
                        className={styles.fullSizeImage}
                    />
                </Modal>
            )}
        </div>
    );
}
