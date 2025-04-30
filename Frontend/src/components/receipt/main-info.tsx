import styles from './main-info.module.scss';
import star from '../../assets/svg/star.svg';
import clock from '../../assets/svg/clock_pink.svg';
import chef from '../../assets/svg/chef_pink.svg';
// import receipt from '../../assets/img/receipt.png';
import { useState } from 'react';
import { Modal } from 'components/common';
import { Recipe } from 'types';
import { transformCategory, transformCookingTime, transformDifficulty, transformRating } from 'utils/transform';

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
    } = recipe;
    const [isModalOpen, setIsModalOpen] = useState(false);

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
                    <p className={styles.tag}>{transformCategory(category)}</p>
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
                            <p className={styles.text}>{transformRating(rating)}</p>
                        </div>
                        <div className={styles.info}>
                            <p className={styles.title}>Время приготовления</p>
                            <img
                                src={clock}
                                alt='clock'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>{transformCookingTime(cookingTime)}</p>
                        </div>
                        <div className={styles.info}>
                            <p className={styles.title}>Сложность</p>
                            <img
                                src={chef}
                                alt='chef'
                                className={styles.img}
                            ></img>
                            <p className={styles.text}>{transformDifficulty(cookingDifficulty)}</p>
                        </div>
                    </div>
                    <p className={styles.description}>{description}</p>
                    <div className={styles.kbju}>
                        <p className={styles.title}>КБЖУ</p>
                        <p>
                            На 100 г:{' '}
                            <span className={styles.text}>
                                {calories} / {proteins} / {fats} / {carbohydrates}
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
