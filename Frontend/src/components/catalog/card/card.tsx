import styles from './card.module.scss';
import clock from '../../../assets/svg/clock.svg';
import chef from '../../../assets/svg/chef.svg';
import fire from '../../../assets/svg/fire.svg';
import { ShortRecipe } from 'types';
import {
    transformDifficulty,
    transformRating,
    transformCookingTime,
} from 'utils/transform';
import { useNavigate } from 'react-router-dom';
import { ButtonWrapper } from 'components/common';
import { favoriteRecipeApi, unfavouriteRecipeApi } from 'store/api';
import fav from '../../../assets/svg/fav.svg';
import unfav from '../../../assets/svg/unfav.svg';
import { useState } from 'react';
import { ReactComponent as CloseIcon } from '../../../assets/svg/ingredient_close.svg';

type CatalogCardProps = {
    recipe: ShortRecipe;
    isEdit?: boolean;
    onDelete?: (id: string) => void;
};

export function CatalogCard(props: CatalogCardProps) {
    const navigate = useNavigate();
    const [isHovered, setIsHovered] = useState(false);

    const { recipe, isEdit, onDelete } = props;
    const {
        id,
        title,
        mainPictureUrl,
        cookingTime,
        cookingDifficulty,
        calories,
        rating,
        isFavourited,
    } = recipe;

    const [favourited, setFavourited] = useState(isFavourited);

    const handleClick = () => {
        navigate(`/recipe/${id}`);
    };

    const handleButtonClick = async () => {
        try {
            if (favourited) {
                await unfavouriteRecipeApi(id);
                setFavourited(false);
            } else {
                await favoriteRecipeApi(id);
                setFavourited(true);
            }
        } catch (error) {
            console.error('Ошибка при обновлении избранного:', error);
        }
    };

    return (
        <div className={styles.container} onClick={handleClick}>
            {isEdit ? (
                <button
                    className={styles.deleteButton}
                    onClick={(e) => {
                        e.stopPropagation();
                        onDelete?.(id);
                    }}
                >
                    <CloseIcon className={styles.close} />
                </button>
            ) : (
                <></>
            )}

            <div
                className={`${styles.rate} 
                ${rating > 8 ? styles.good : rating < 4 ? styles.bad : ''}
                `}
            >
                {transformRating(rating)}
            </div>
            <img src={mainPictureUrl} alt='porridge' className={styles.image} />
            <div className={styles.description}>
                <div className={styles.name}>
                    <p className={styles.title}>{title}</p>
                    <ButtonWrapper onAuthenticatedAction={handleButtonClick}>
                        <button
                            className={styles.icon}
                            onMouseEnter={() => setIsHovered(true)}
                            onMouseLeave={() => setIsHovered(false)}
                            onClick={(e) => {
                                e.stopPropagation();
                                handleButtonClick();
                            }}
                        >
                            <img
                                src={fav}
                                alt='fav icon'
                                className={`${styles.iconImage} ${
                                    favourited && !isHovered
                                        ? styles.visible
                                        : favourited && isHovered
                                        ? styles.hidden
                                        : !favourited && isHovered
                                        ? styles.visible
                                        : styles.hidden
                                }`}
                            />
                            <img
                                src={unfav}
                                alt='unfav icon'
                                className={`${styles.iconImage} ${
                                    favourited && isHovered
                                        ? styles.visible
                                        : favourited && !isHovered
                                        ? styles.hidden
                                        : !favourited && !isHovered
                                        ? styles.visible
                                        : styles.hidden
                                }`}
                            />
                        </button>
                    </ButtonWrapper>
                </div>
                <div className={styles.infoContainer}>
                    <p className={styles.info}>
                        <img
                            src={clock}
                            alt='clock'
                            className={styles.infoIcon}
                        />
                        {transformCookingTime(cookingTime)}
                    </p>
                    <p className={styles.info}>
                        <img
                            src={chef}
                            alt='chef'
                            className={styles.infoIcon}
                        />
                        {transformDifficulty(cookingDifficulty)}
                    </p>
                    <p className={styles.info}>
                        <img
                            src={fire}
                            alt='fire'
                            className={styles.infoIcon}
                        />
                        {calories} ккал
                    </p>
                </div>
            </div>
        </div>
    );
}
