/* eslint-disable @typescript-eslint/no-non-null-assertion */
import { CATEGORY, DIFFICULTY, MEASURE, TAG } from 'mocks/filter';
import styles from './edit-recipe.module.scss';
import { ReactComponent as CloseIcon } from '../../assets/svg/ingredient_close.svg';
import plus from '../../assets/svg/ingredient_plus.svg';
import stars from '../../assets/svg/ingredient_stars.svg';
import { useEffect, useState } from 'react';
import { CookingStep, Ingredient, Measure, Recipe } from 'types';
import { useRef } from 'react';
import { useDispatch, useSelector } from 'store/store';
import { selectUser } from 'store/user.slice';
import {
    createRecipe,
    CreateRecipeDto,
    getRecipeByIdApi,
    updateRecipe,
    UpdateRecipeDto,
} from 'store/api';
import { useNavigate, useParams } from 'react-router-dom';
import ScrollToTop from 'components/common/scrollToTop';
import { Helmet } from 'react-helmet-async';

export function EditRecipePage() {
    const { id } = useParams<{ id: string }>();
    const dispatch = useDispatch();
    const navigate = useNavigate();
    const [recipe, setRecipe] = useState<Recipe | null>(null);
    const [ingredients, setIngredients] = useState<Ingredient[]>([
        { name: '', numericValue: 0, measure: Measure.gram },
    ]);

    const [steps, setSteps] = useState<CookingStep[]>([
        { title: '', description: '', step: 1 },
    ]);
    const [existingPhoto, setExistingPhoto] = useState<{ url: string } | null>(
        null
    );
    const [imagePreview, setImagePreview] = useState<string | null>(null);

    const titleRef = useRef<HTMLInputElement>(null);
    const descriptionRef = useRef<HTMLTextAreaElement>(null);
    const categoryRef = useRef<HTMLSelectElement>(null);
    const tagRef = useRef<HTMLSelectElement>(null);
    const timeRef = useRef<HTMLInputElement>(null);
    const difficultyRef = useRef<HTMLInputElement[]>([]);
    const photoRef = useRef<HTMLInputElement>(null);

    const [title, setTitle] = useState('');

    const user = useSelector(selectUser);

    useEffect(() => {
        if (id) {
            getRecipeByIdApi(id).then((fetchedRecipe) => {
                setRecipe(fetchedRecipe);
                setIngredients(fetchedRecipe.ingredients);
                setSteps(fetchedRecipe.cookingSteps);
            });
            setTitle('Редактирование рецепта');
        } else {
            setIngredients([
                { name: '', numericValue: 0, measure: Measure.gram },
            ]);
            setSteps([{ title: '', description: '', step: 1 }]);
            setTitle('Создание рецепта');
        }
    }, [id, dispatch]);

    useEffect(() => {
        if (recipe) {
            setExistingPhoto(recipe.picturesUrls[0]);
            if (recipe.picturesUrls[0]?.url) {
                setImagePreview(recipe.picturesUrls[0].url);
            }
        }
    }, [recipe]);

    useEffect(() => {
        if (recipe) {
            titleRef.current!.value = recipe.title;
            descriptionRef.current!.value = recipe.description;
            categoryRef.current!.value = recipe.category;
            timeRef.current!.value = String(recipe.cookingTime);
            photoRef.current!.value = '';

            recipe.tags.forEach((tag) => {
                const option = tagRef.current?.querySelector(
                    `option[value="${tag}"]`
                ) as HTMLOptionElement;
                if (option) option.selected = true;
            });

            difficultyRef.current.forEach((input) => {
                if (input.value === recipe.cookingDifficulty) {
                    input.checked = true;
                }
            });
        }
    }, [recipe]);

    function autoResize(e: React.ChangeEvent<HTMLTextAreaElement>) {
        e.target.style.height = 'auto';
        e.target.style.height = `${e.target.scrollHeight}px`;
    }

    const handleIngredientChange = <K extends keyof Ingredient>(
        index: number,
        field: K,
        value: Ingredient[K] extends number ? number : string
    ) => {
        const newIngredients = [...ingredients];
        newIngredients[index][field] = value as Ingredient[K];
        setIngredients(newIngredients);
    };

    const handleStepChange = <K extends keyof CookingStep>(
        index: number,
        field: K,
        value: CookingStep[K] extends number ? number : string
    ) => {
        const newSteps = [...steps];
        newSteps[index][field] = value as CookingStep[K];
        setSteps(newSteps);
    };

    const addIngredient = () => {
        setIngredients([
            ...ingredients,
            { name: '', numericValue: 0, measure: Measure.gram },
        ]);
    };

    const removeIngredient = (index: number) => {
        setIngredients(ingredients.filter((_, i) => i !== index));
    };

    const addStep = () => {
        setSteps([
            ...steps,
            { title: '', description: '', step: steps.length + 1 },
        ]);
    };

    const removeStep = (index: number) => {
        setSteps(steps.filter((_, i) => i !== index));
    };

    const handleSave = async () => {
        try {
            const selectedDifficulty = difficultyRef.current.find(
                (input) => input.checked
            )?.value;

            const file = photoRef.current?.files?.[0];

            if (!file && !existingPhoto) {
                throw new Error('Не выбрано изображение для рецепта');
            }

            const baseRecipe: CreateRecipeDto = {
                title: titleRef.current?.value || '',
                description: descriptionRef.current?.value || '',
                tags: Array.from(tagRef.current?.selectedOptions || []).map(
                    (opt) => opt.value
                ),
                category: categoryRef.current?.value || '',
                cookingDifficulty: selectedDifficulty || '',
                cookingTime: Number(timeRef.current?.value || 0),
                ingredients: ingredients.map((i) => ({
                    name: i.name,
                    numericValue: Number(i.numericValue),
                    measure: i.measure,
                })),
                cookingSteps: steps.map((s, i) => ({
                    step: i + 1,
                    title: s.title,
                    description: s.description,
                })),
                userId: user!.id || '',
                picturesUrls: existingPhoto ? [existingPhoto] : [],
            };
            if (!id) {
                const recipeId = await createRecipe(baseRecipe, file);
                navigate(`/recipe/${recipeId}`);
            } else {
                if (!recipe) return;

                const changedFields = getChangedFields(recipe, baseRecipe);
                await updateRecipe(recipe.id, changedFields, file);
                navigate(`/recipe/${id}`);
            }
        } catch (err) {
            alert((err as Error).message);
        }
    };

    function getChangedFields(
        original: Recipe,
        updated: CreateRecipeDto
    ): UpdateRecipeDto {
        const changed: UpdateRecipeDto = { userId: '' };

        if (original.title !== updated.title) changed.title = updated.title;
        if (original.description !== updated.description)
            changed.description = updated.description;
        if (original.category !== updated.category)
            changed.category = updated.category;
        if (original.cookingDifficulty !== updated.cookingDifficulty)
            changed.cookingDifficulty = updated.cookingDifficulty;
        if (original.cookingTime !== updated.cookingTime)
            changed.cookingTime = updated.cookingTime;

        if (JSON.stringify(original.tags) !== JSON.stringify(updated.tags)) {
            changed.tags = updated.tags;
        }

        if (
            JSON.stringify(original.ingredients) !==
            JSON.stringify(updated.ingredients)
        ) {
            changed.ingredients = updated.ingredients;
        }

        if (
            JSON.stringify(original.cookingSteps) !==
            JSON.stringify(updated.cookingSteps)
        ) {
            changed.cookingSteps = updated.cookingSteps;
        }

        if (updated.userId) {
            changed.userId = updated.userId;
        }

        return changed;
    }

    return (
        <>
            <ScrollToTop />
            <Helmet>
                <title>{title}</title>
            </Helmet>
            <div className={styles.background}>
                <div className={styles.mainContainer}>
                    <div className={styles.backContainer}>
                        <button
                            className={styles.back}
                            onClick={() => {
                                navigate(-1);
                            }}
                        >
                            <span className={styles.arrow}></span>Назад
                        </button>
                    </div>
                    <div className={styles.content}>
                        <h1 className={styles.h1}>Создание рецепта</h1>
                        <div className={styles.container}>
                            <div className={styles.infoContainer}>
                                <h2 className={styles.h2}>Общая информация</h2>
                                <div className={styles.inputContainer}>
                                    <label
                                        className={styles.label}
                                        htmlFor='title'
                                    >
                                        Название
                                    </label>
                                    <input
                                        ref={titleRef}
                                        type='text'
                                        id='title'
                                        className={styles.input}
                                        placeholder='Шарлотка'
                                        defaultValue={recipe?.title}
                                        maxLength={50}
                                    />
                                </div>
                                <div className={styles.inputContainer}>
                                    <label className={styles.label}>
                                        Фотография
                                    </label>

                                    <div
                                        className={`${styles.input} ${styles.photo}`}
                                    >
                                        {imagePreview && (
                                            <img
                                                src={imagePreview}
                                                alt='Превью рецепта'
                                                className={styles.imagePreview}
                                            />
                                        )}

                                        <label
                                            className={`button ${styles.uploadButton}`}
                                        >
                                            Загрузить фото
                                            <input
                                                type='file'
                                                accept='image/*'
                                                className={styles.hiddenInput}
                                                ref={photoRef}
                                                onChange={(e) => {
                                                    const file =
                                                        e.target.files?.[0];
                                                    if (file) {
                                                        setImagePreview(
                                                            URL.createObjectURL(
                                                                file
                                                            )
                                                        );
                                                    }
                                                }}
                                            />
                                        </label>
                                    </div>
                                </div>
                                <div className={styles.inputContainer}>
                                    <label
                                        className={styles.label}
                                        htmlFor='description'
                                    >
                                        Описание
                                    </label>
                                    <textarea
                                        ref={descriptionRef}
                                        id='description'
                                        className={`${styles.input} ${styles.textarea}`}
                                        maxLength={320}
                                        value={recipe?.description}
                                        placeholder='Введите краткое описание рецепта (до 320 символов)'
                                    />
                                </div>
                                <div className={styles.inputContainer}>
                                    <label
                                        className={styles.label}
                                        htmlFor='category'
                                    >
                                        Категория
                                    </label>
                                    <select
                                        id='category'
                                        className={styles.input}
                                        ref={categoryRef}
                                    >
                                        {CATEGORY.map((cat) => (
                                            <option
                                                key={cat.value}
                                                value={cat.value}
                                            >
                                                {cat.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className={styles.inputContainer}>
                                    <label
                                        className={styles.label}
                                        htmlFor='tag'
                                    >
                                        Теги
                                    </label>
                                    <select
                                        ref={tagRef}
                                        id='tag'
                                        className={`${styles.input} ${styles.tag}`}
                                        multiple
                                    >
                                        {TAG.map((tag) => (
                                            <option
                                                key={tag.value}
                                                value={tag.value}
                                            >
                                                {tag.name}
                                            </option>
                                        ))}
                                    </select>
                                </div>
                                <div className={styles.inputContainer}>
                                    <label
                                        className={styles.label}
                                        htmlFor='difficulty'
                                    >
                                        Сложность
                                    </label>
                                    <div className={styles.difficulty}>
                                        {DIFFICULTY.map((diff) => (
                                            <label
                                                key={diff.value}
                                                className={styles.radioLabel}
                                            >
                                                <input
                                                    ref={(el) => {
                                                        if (el)
                                                            difficultyRef.current.push(
                                                                el
                                                            );
                                                    }}
                                                    type='radio'
                                                    name='difficulty'
                                                    value={diff.value}
                                                />
                                                {diff.name}
                                            </label>
                                        ))}
                                    </div>
                                </div>
                                <div className={styles.inputContainer}>
                                    <label
                                        className={styles.label}
                                        htmlFor='time'
                                    >
                                        Время (в минутах)
                                    </label>
                                    <input
                                        ref={timeRef}
                                        type='number'
                                        id='time'
                                        className={styles.input}
                                        placeholder='20'
                                        min={1}
                                        max={9999}
                                    />
                                </div>
                                {/* <div className={styles.inputContainer}>
                                <label
                                    className={styles.label}
                                    htmlFor='calories'
                                >
                                    КБЖУ на 100 г
                                </label>
                                <div className={styles.kbjuContainer}>
                                    {[
                                        'Калории',
                                        'Белки',
                                        'Жиры',
                                        'Углеводы',
                                    ].map((label, idx) => (
                                        <div key={idx} className={styles.kbju}>
                                            <input
                                                ref={(el) => {
                                                    if (el)
                                                        caloriesRef.current[
                                                            idx
                                                        ] = el;
                                                }}
                                                type='number'
                                                className={styles.input}
                                                placeholder={label}
                                            />
                                            <span>
                                                {label === 'Калории'
                                                    ? 'ккал'
                                                    : label[0].toLowerCase()}
                                            </span>
                                        </div>
                                    ))}
                                </div>
                            </div> */}
                            </div>
                            <div className={styles.infoContainer}>
                                <h2 className={styles.h2}>Ингредиенты</h2>

                                {ingredients.map((ingredient, index) => (
                                    <div
                                        key={index}
                                        className={styles.ingredientContainer}
                                    >
                                        <input
                                            type='text'
                                            placeholder='Название'
                                            value={ingredient.name}
                                            className={styles.input}
                                            onChange={(e) =>
                                                handleIngredientChange(
                                                    index,
                                                    'name',
                                                    e.target.value
                                                )
                                            }
                                            maxLength={30}
                                        />
                                        <input
                                            type='number'
                                            placeholder='Количество'
                                            className={`${styles.input} ${styles.number}`}
                                            min='0'
                                            value={ingredient.numericValue}
                                            onChange={(e) =>
                                                handleIngredientChange(
                                                    index,
                                                    'numericValue',
                                                    Number(e.target.value)
                                                )
                                            }
                                        />
                                        <select
                                            value={ingredient.measure}
                                            className={`${styles.input} ${styles.number}`}
                                            onChange={(e) =>
                                                handleIngredientChange(
                                                    index,
                                                    'measure',
                                                    e.target.value
                                                )
                                            }
                                        >
                                            <option value=''>Ед. изм</option>
                                            {MEASURE.map((m) => (
                                                <option
                                                    key={m.value}
                                                    value={m.value}
                                                >
                                                    {m.name}
                                                </option>
                                            ))}
                                        </select>
                                        <button
                                            className={styles.buttonDelete}
                                            onClick={() =>
                                                removeIngredient(index)
                                            }
                                        >
                                            <CloseIcon
                                                className={styles.icon}
                                            />
                                        </button>
                                    </div>
                                ))}
                                <div className={styles.buttons}>
                                    <button
                                        className={styles.buttonPlus}
                                        onClick={addIngredient}
                                    >
                                        <img src={plus} alt='plus ingredient' />
                                        Добавить ингридиент
                                    </button>
                                </div>
                            </div>
                            <div className={styles.infoContainer}>
                                <h2 className={styles.h2}>
                                    Шаги приготовления
                                </h2>

                                {steps.map((step, index) => (
                                    <div
                                        key={index}
                                        className={styles.stepContainer}
                                    >
                                        <div className={styles.step}>
                                            <p>Шаг {index + 1}</p>
                                            <input
                                                type='text'
                                                placeholder='Название шага'
                                                className={styles.input}
                                                value={step.title}
                                                onChange={(e) =>
                                                    handleStepChange(
                                                        index,
                                                        'title',
                                                        e.target.value
                                                    )
                                                }
                                            />
                                            <button
                                                className={styles.buttonDelete}
                                                onClick={() =>
                                                    removeStep(index)
                                                }
                                            >
                                                <CloseIcon
                                                    className={styles.icon}
                                                />
                                            </button>
                                        </div>

                                        <textarea
                                            placeholder='Описание'
                                            value={step.description}
                                            className={`${styles.input} ${styles.textareaStep}`}
                                            onChange={(e) => {
                                                handleStepChange(
                                                    index,
                                                    'description',
                                                    e.target.value
                                                );
                                                autoResize(e);
                                            }}
                                        />
                                    </div>
                                ))}

                                <div className={styles.buttons}>
                                    <button
                                        className={styles.buttonPlus}
                                        onClick={addStep}
                                    >
                                        <img src={plus} alt='plus ingredient' />
                                        Добавить шаг
                                    </button>
                                </div>
                            </div>
                            <button
                                className={styles.buttonSave}
                                onClick={handleSave}
                            >
                                <img src={stars} alt='stars' />
                                Сохранить изменения
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
}
