import { LayoutHeader } from 'components/layout';
import styles from './coll-header.module.scss';
import { Toggle } from 'components/common';
import fav from '../../../assets/svg/coll_fav_dark.svg';
import edit from '../../../assets/svg/edit.svg';
import { Collection } from 'types/collections.type';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';
import {
    deleteCollectionApi,
    getCollectionRateApi,
    putRateCollectionApi,
    updateCollectionApi,
} from 'store/api';
import { EditHeader } from './edit-coll-header';
import { COLORS } from 'mocks/colors';
import { useDispatch, useSelector } from 'store/store';
import { fetchUsersCollections, selectUser } from 'store/user.slice';
import { Ratings } from 'components/receipt';

type props = {
    collection: Collection;
};

export type editingType = {
    title: string;
    color: string;
    isPrivate: boolean;
};

export function MyCollHeader(props: props) {
    const { collection } = props;
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const user = useSelector(selectUser);
    const [isDeleting, setIsDeleting] = useState(false);
    const [isEditing, setIsEditing] = useState(false);
    const [editCollection, setEditCollection] = useState<editingType>({
        title: collection.title,
        color: collection.color,
        isPrivate: collection.isPrivate,
    });
    const [userRating, setUserRating] = useState(0);
    const [isRatingLoading, setIsRatingLoading] = useState(false);

    useEffect(() => {
        const loadUserRating = async () => {
            try {
                const ratingData = await getCollectionRateApi(collection.id);
                setUserRating(ratingData.rate);
            } catch (error) {
                console.error('Ошибка загрузки оценки:', error);
            }
        };

        loadUserRating();
    }, [collection.id]);

    const handleRateCollection = async (rating: number) => {
        try {
            setIsRatingLoading(true);
            await putRateCollectionApi(collection.id, rating);
            setUserRating(rating);
        } catch (error) {
            console.error('Ошибка при сохранении оценки:', error);
        } finally {
            setIsRatingLoading(false);
        }
    };

    const handleShare = () => {
        const shareData = {
            title: collection.title,
            text: 'Посмотри эту потрясающую подборку рецептов!',
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

    const handleDelete = async () => {
        if (
            !window.confirm(
                `Вы уверены, что хотите удалить подборку "${collection.title}"?`
            )
        ) {
            return;
        }

        try {
            setIsDeleting(true);
            await deleteCollectionApi(collection.id);
            dispatch(fetchUsersCollections(user!.id));
            navigate(-1);
        } catch (error) {
            alert((error as Error).message || 'Ошибка при удалении подборки');
        } finally {
            setIsDeleting(false);
        }
    };

    const handleToggleChange = async (newPrivate: boolean) => {
        try {
            await updateCollectionApi(collection.id, {
                isPrivate: newPrivate,
            });
            setEditCollection((prev) => ({ ...prev, isPrivate: newPrivate }));
        } catch (error) {
            alert('Ошибка при изменении состояния публикации');
        }
    };

    const handleEdit = () => {
        if (collection.title !== 'Избранное') {
            setIsEditing(true);
        }
    };

    const handleCloseEdit = () => {
        setIsEditing(false);
    };

    const handleSave = (coll: editingType) => {
        setIsEditing(false);
        window.location.reload();
        setEditCollection({ ...coll });
    };

    return (
        <div className={styles.mainContainer}>
            <LayoutHeader>
                <div className={styles.container}>
                    {isEditing ? (
                        <EditHeader
                            collection={collection}
                            onClose={handleCloseEdit}
                            onSave={handleSave}
                        />
                    ) : (
                        <div className={styles.header}>
                            <div className={styles.info}>
                                <div className={styles.name}>
                                    <p className={styles.title}>
                                        {editCollection.title}
                                    </p>
                                    <div
                                        className={styles.color}
                                        style={{
                                            backgroundColor: COLORS.find(
                                                (item) =>
                                                    item.value ===
                                                    editCollection.color
                                            )?.color,
                                        }}
                                    ></div>
                                </div>

                                <div className={styles.buttons}>
                                    {collection.title !== 'Избранное' ? (
                                        <>
                                            <button
                                                className={styles.button}
                                                onClick={handleEdit}
                                            >
                                                <img src={edit} alt='edit' />
                                                Изм.
                                            </button>
                                            <div className={styles.publish}>
                                                <Toggle
                                                    isActive={
                                                        !editCollection.isPrivate
                                                    }
                                                    onToggleChange={
                                                        handleToggleChange
                                                    }
                                                />
                                                <span>Опубликовать</span>
                                            </div>
                                            <div className={styles.fav}>
                                                <img
                                                    src={fav}
                                                    alt='favorite'
                                                    className={styles.icon}
                                                />
                                                <span>
                                                    {collection.popularity}
                                                </span>
                                            </div>
                                        </>
                                    ) : (
                                        <></>
                                    )}
                                </div>
                            </div>
                            {collection.title !== 'Избранное' && (
                                <Ratings
                                    currentRating={userRating}
                                    onRate={handleRateCollection}
                                    disabled={isRatingLoading}
                                    title={'подборку'}
                                />
                            )}

                            <div className={styles.shareContainer}>
                                <button
                                    className='button'
                                    onClick={handleShare}
                                >
                                    <span
                                        className={`icon ${styles.share}`}
                                    ></span>
                                    Поделиться
                                </button>
                                {collection.title !== 'Избранное' ? (
                                    <button
                                        className={`button ${styles.deleteButton} `}
                                        onClick={handleDelete}
                                        disabled={isDeleting}
                                    >
                                        <span
                                            className={`icon ${styles.delete}`}
                                        ></span>
                                        Удалить подборку
                                    </button>
                                ) : (
                                    <></>
                                )}
                            </div>
                        </div>
                    )}
                </div>
            </LayoutHeader>
        </div>
    );
}
