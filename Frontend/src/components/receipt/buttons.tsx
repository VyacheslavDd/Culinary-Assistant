import { ButtonWrapper } from 'components/common';
import styles from './buttons.module.scss';
import { useEffect, useRef, useState } from 'react';
import { useSelector } from 'store/store';
import { selectIsAuthenticated, selectUserCollections } from 'store/user.slice';
import { AddToCollectionOverlay } from 'components/common/add-to-collection/add-to-collection';
import {
    addRecipesCollectionApi,
    favoriteRecipeApi,
    unfavouriteRecipeApi,
} from 'store/api';
import { ReactComponent as Favorite } from '../../assets/svg/fav.svg';
import { ReactComponent as Unfavorite } from '../../assets/svg/unfav.svg';

type props = {
    name: string;
    recipeId: string;
    isFavourite: boolean;
};

export function ButtonsReceipt(props: props) {
    const { name, recipeId, isFavourite: initialFavourite } = props;
    const isAuth = useSelector(selectIsAuthenticated);
    const [showCollectionOverlay, setShowCollectionOverlay] = useState(false);
    const [isFavourite, setIsFavourite] = useState(initialFavourite);
    const [isLoading, setIsLoading] = useState(false);

    const collections = useSelector(selectUserCollections);

    useEffect(() => {
        setIsFavourite(initialFavourite);
    }, [initialFavourite]);

    const handleAddToCollection = () => {
        setShowCollectionOverlay(!showCollectionOverlay);
    };

    const handleFavourite = async () => {
        if (isLoading) return;

        setIsLoading(true);
        try {
            if (isFavourite) {
                await unfavouriteRecipeApi(recipeId);
                setIsFavourite(false);
            } else {
                await favoriteRecipeApi(recipeId);
                setIsFavourite(true);
            }
        } catch (error) {
            console.error('Ошибка при изменении статуса избранного:', error);
        } finally {
            setIsLoading(false);
        }
    };

    const handleSelectCollection = async (collectionId: string) => {
        try {
            await addRecipesCollectionApi(collectionId, {
                receipts: [recipeId],
            });
            setShowCollectionOverlay(false);
        } catch (error) {
            console.error('Error adding recipe to collection:', error);
        }
    };

    const handleShare = () => {
        const shareData = {
            title: name,
            text: 'Посмотри этот потрясающий рецепт!',
            url: window.location.href,
        };

        if (navigator.share) {
            navigator
                .share(shareData)
                .catch((err) =>
                    console.error('Ошибка при попытке поделиться:', err)
                );
        } else {
            navigator.clipboard
                .writeText(
                    `${shareData.title}
                ${shareData.url}`
                )
                .then(() => {
                    alert('Ссылка скопирована в буфер обмена!');
                });
        }
    };

    return (
        <div className={styles.mainContainer}>
            {isAuth && (
                <ButtonWrapper onAuthenticatedAction={handleFavourite}>
                    <button
                        className={`button ${styles.buttonShare}`}
                        disabled={isLoading}
                    >
                        {isFavourite ? (
                            <Favorite className={styles.icon} />
                        ) : (
                            <Unfavorite className={styles.icon} />
                        )}
                        {isFavourite ? 'Не нравится' : 'Нравится'}
                    </button>
                </ButtonWrapper>
            )}

            <button
                className={`button ${styles.buttonShare}`}
                onClick={handleShare}
            >
                <span className={`${styles.icon} ${styles.share}`}></span>
                Поделиться
            </button>

            {isAuth && (
                <ButtonWrapper onAuthenticatedAction={handleAddToCollection}>
                    <button className={styles.buttonAdd}>
                        <span className={`${styles.icon} ${styles.add}`}></span>
                        Добавить в подборку
                        {showCollectionOverlay && (
                            <AddToCollectionOverlay
                                collections={collections}
                                onSelectCollection={handleSelectCollection}
                            />
                        )}
                    </button>
                </ButtonWrapper>
            )}
        </div>
    );
}
