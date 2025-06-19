import styles from './coll-header.module.scss';
import { Collection } from 'types/collections.type';
import check from '../../../../assets/svg/check_black.svg';
import close from '../../../../assets/svg/cancel.svg';
import { COLORS } from 'mocks/colors';
import { useState } from 'react';
import { Color } from 'types/color.enum';
import { updateCollectionApi } from 'store/api';
import { editingType } from './my-coll-header';

type props = {
    collection: Collection;
    onClose: () => void;
    onSave: (coll: editingType) => void;
};

export function EditHeader(props: props) {
    const { collection, onClose, onSave } = props;

    const [originalState, setOriginalState] = useState<editingType>({
        title: collection.title,
        color: collection.color,
        isPrivate: collection.isPrivate,
    });

    const [edit, setEdit] = useState<editingType>(originalState);

    const handleChangeTitle = (e: React.ChangeEvent<HTMLInputElement>) => {
        setEdit((prevEdit) => ({
            ...prevEdit,
            title: e.target.value,
        }));
    };

    const handleChangeColor = (color: Color) => {
        setEdit((prevEdit) => ({
            ...prevEdit,
            color,
        }));
    };

    const handleCancel = () => {
        setEdit(originalState);
        onClose();
    };

    const handleSave = async () => {
        try {
            await updateCollectionApi(collection.id, {
                title: edit.title,
                color: edit.color,
                isPrivate: edit.isPrivate,
            });
            setOriginalState(edit);

            onSave(edit);
        } catch (error) {
            alert('Failed to update collection');
        }
    };

    return (
        <div className={styles.containerEdit}>
            <div className={styles.edit}>
                <div className={styles.inputContainer}>
                    <label>Название: </label>
                    <input
                        type='text'
                        className={styles.input}
                        value={edit.title}
                        onChange={handleChangeTitle}
                    />
                </div>
                <div className={styles.inputContainer}>
                    <label>Цвет: </label>
                    <ul className={styles.colors}>
                        {COLORS.map((color) => (
                            <li key={color.value}>
                                <button
                                    className={`${styles.button} ${
                                        edit.color === color.value
                                            ? styles.selected
                                            : ''
                                    }`}
                                    style={{ backgroundColor: color.color }}
                                    onClick={() =>
                                        handleChangeColor(color.value as Color)
                                    }
                                ></button>
                            </li>
                        ))}
                    </ul>
                </div>
            </div>
            <div className={styles.buttons}>
                <button className={styles.button} onClick={handleSave}>
                    <img src={check} alt='check' />
                    Подтвердить
                </button>

                <button
                    className={`${styles.button} ${styles.close}`}
                    onClick={handleCancel}
                >
                    <img src={close} alt='check' />
                    Отменить
                </button>
            </div>
        </div>
    );
}
